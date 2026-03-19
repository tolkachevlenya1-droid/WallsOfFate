using UnityEngine;

public class RouteGridOccupant : MonoBehaviour
{
    public Vector2Int GridPosition;
    public RouteCellType CellType = RouteCellType.Argument;

    [Header("Grid Position")]
    [SerializeField] private bool derivePositionFromTransform = true;
    [SerializeField] private GridManager grid;

    [Header("Optional Visuals")]
    [SerializeField] private GameObject visualRoot;
    [SerializeField] private Renderer[] tintedRenderers;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private bool autoTint = true;
    [SerializeField] private bool hideWhenConsumed = true;
    [SerializeField] private Color wallColor = new(0.18f, 0.18f, 0.18f, 1f);
    [SerializeField] private Color argumentColor = new(0.95f, 0.8f, 0.2f, 1f);
    [SerializeField] private Color exitColor = new(0.2f, 0.8f, 0.35f, 1f);
    [SerializeField] private Color forbiddenColor = new(0.8f, 0.2f, 0.2f, 1f);

    private bool _consumed;

    public bool BlocksMovement => CellType == RouteCellType.Wall;
    public bool IsForbidden => CellType == RouteCellType.Forbidden;
    public bool IsExit => CellType == RouteCellType.Exit;
    public bool HasAvailableArgument => CellType == RouteCellType.Argument && !_consumed;

    public void SyncGridPosition(GridManager fallbackGrid = null)
    {
        if (!derivePositionFromTransform)
        {
            return;
        }

        GridManager targetGrid = grid != null ? grid : fallbackGrid;
        if (targetGrid == null)
        {
            targetGrid = FindObjectOfType<GridManager>(true);
        }

        if (targetGrid != null && targetGrid.TryGetGridPositionFromWorld(transform.position, out Vector2Int resolvedPosition))
        {
            GridPosition = resolvedPosition;
        }
    }

    public void ResetState()
    {
        _consumed = false;

        if (visualRoot == null)
        {
            visualRoot = gameObject;
        }

        if (hideWhenConsumed)
        {
            visualRoot.SetActive(true);
        }

        RefreshVisual();
    }

    public bool TryCollectArgument()
    {
        if (!HasAvailableArgument)
        {
            return false;
        }

        _consumed = true;

        if (visualRoot == null)
        {
            visualRoot = gameObject;
        }

        if (hideWhenConsumed)
        {
            visualRoot.SetActive(false);
        }

        RefreshVisual();
        return true;
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
            RouteCellType.Argument => argumentColor,
            RouteCellType.Exit => exitColor,
            RouteCellType.Forbidden => forbiddenColor,
            _ => Color.white
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
        SyncGridPosition();

        if (visualRoot == null)
        {
            visualRoot = gameObject;
        }

        ResetState();
    }

    private void OnValidate()
    {
        SyncGridPosition();
        RefreshVisual();
    }
}
