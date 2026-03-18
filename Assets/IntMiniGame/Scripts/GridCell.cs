using UnityEngine;

public class GridCell : MonoBehaviour
{
    public Vector2Int GridPosition;
    public RouteCellType CellType = RouteCellType.Normal;

    [Header("Optional Visuals")]
    [SerializeField] private GameObject collectibleVisual;
    [SerializeField] private Renderer[] tintedRenderers;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private bool autoTint = true;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color wallColor = new(0.2f, 0.2f, 0.2f, 1f);
    [SerializeField] private Color argumentColor = new(1f, 0.8f, 0.15f, 1f);
    [SerializeField] private Color exitColor = new(0.25f, 0.8f, 0.35f, 1f);
    [SerializeField] private Color forbiddenColor = new(0.8f, 0.2f, 0.2f, 1f);
    [SerializeField] private Color collectedArgumentColor = new(0.7f, 0.7f, 0.7f, 1f);

    private bool _argumentCollected;

    public bool HasAvailableArgument => CellType == RouteCellType.Argument && !_argumentCollected;

    public void ResetState()
    {
        _argumentCollected = false;

        if (collectibleVisual != null)
        {
            collectibleVisual.SetActive(CellType == RouteCellType.Argument);
        }

        RefreshVisual();
    }

    public bool TryCollectArgument()
    {
        if (!HasAvailableArgument)
        {
            return false;
        }

        _argumentCollected = true;

        if (collectibleVisual != null)
        {
            collectibleVisual.SetActive(false);
        }

        RefreshVisual();
        return true;
    }

    public bool IsBlocked()
    {
        return CellType == RouteCellType.Wall;
    }

    public bool IsForbidden()
    {
        return CellType == RouteCellType.Forbidden;
    }

    public bool IsExit()
    {
        return CellType == RouteCellType.Exit;
    }

    public void RefreshVisual()
    {
        if (!autoTint)
        {
            return;
        }

        Color targetColor = CellType switch
        {
            RouteCellType.Wall => wallColor,
            RouteCellType.Argument => _argumentCollected ? collectedArgumentColor : argumentColor,
            RouteCellType.Exit => exitColor,
            RouteCellType.Forbidden => forbiddenColor,
            _ => normalColor
        };

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.color = targetColor;
        }

        if (tintedRenderers == null || tintedRenderers.Length == 0)
        {
            tintedRenderers = GetComponentsInChildren<Renderer>(true);
        }

        for (int index = 0; index < tintedRenderers.Length; index++)
        {
            Renderer targetRenderer = tintedRenderers[index];
            if (targetRenderer == null || targetRenderer.sharedMaterial == null || !targetRenderer.sharedMaterial.HasProperty("_Color"))
            {
                continue;
            }

            targetRenderer.material.color = targetColor;
        }
    }

    private void Awake()
    {
        ResetState();
    }

    private void OnValidate()
    {
        RefreshVisual();
    }
}
