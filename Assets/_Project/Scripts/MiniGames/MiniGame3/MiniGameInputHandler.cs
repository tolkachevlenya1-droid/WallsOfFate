using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MiniGameInputHandler : MonoBehaviour
{
    public CommandQueue queue;
    public ExecutionManager executor;

    [Header("Input Mapping")]
    [SerializeField] private bool swapHorizontalControls;
    [SerializeField] private bool swapVerticalControls;

    [Header("Board Queue Sprites")]
    [SerializeField] private Sprite queueArrowSprite;
    [SerializeField] private Sprite queuePauseSprite;
    [SerializeField] private Color queueIconColor = new(0.96f, 0.84f, 0.55f, 1f);
    [SerializeField] private Color queueFailureColor = new(0.9f, 0.3f, 0.3f, 1f);
    [SerializeField] private float queueArrowBaseRotation;
    [SerializeField, Range(1, 9)] private int queueIconsPerRow = 5;
    [SerializeField, Range(0.1f, 1f)] private float queueContentWidthFactor = 0.8f;
    [SerializeField, Range(0.1f, 1f)] private float queueContentHeightFactor = 0.62f;

    [Header("Board Queue Limit")]
    [SerializeField] private Text queueLimitTextOverride;
    [SerializeField] private TMP_Text queueLimitTextMeshOverride;

    [Header("Attempts UI")]
    [SerializeField] private Image[] attemptHeartImages;
    [SerializeField] private SpriteRenderer[] attemptHeartSpriteRenderers;
    [SerializeField] private Sprite attemptHeartFilledSprite;
    [SerializeField] private Sprite attemptHeartSpentSprite;

    private RouteMiniGameHUD _hud;
    private readonly Dictionary<RouteControlAction, SpriteRenderer> _boardButtonSprites = new();
    private readonly Dictionary<RouteControlAction, Vector3> _boardButtonScales = new();
    private readonly Dictionary<RouteControlAction, Coroutine> _boardButtonFlashRoutines = new();
    private readonly Dictionary<Collider, RouteControlAction> _boardButtonColliders = new();
    private readonly Dictionary<RouteControlAction, Button> _boardUiButtons = new();
    private readonly Dictionary<RouteControlAction, Image> _boardUiButtonImages = new();
    private readonly Dictionary<RouteControlAction, RectTransform> _boardUiButtonRects = new();

    private TextMesh _boardQueueText;
    private Transform _boardQueueBox;
    private Transform _boardQueueIconsRoot;
    private RectTransform _boardQueueUiRoot;
    private Text _boardQueueLimitText;
    private Camera _boardCamera;
    private bool _hasBoardUi;
    private readonly List<SpriteRenderer> _boardQueueIconRenderers = new();
    private readonly List<TextMesh> _boardQueueLabelRenderers = new();
    private readonly List<Image> _boardQueueUiImages = new();
    private readonly List<Text> _boardQueueUiLabels = new();
    private Sprite[] _attemptHeartImageDefaults = Array.Empty<Sprite>();
    private Sprite[] _attemptHeartRendererDefaults = Array.Empty<Sprite>();

    private static readonly Color BoardButtonIdleColor = Color.white;
    private static readonly Color BoardButtonActiveColor = new(1f, 0.86f, 0.55f, 1f);
    private static readonly Vector3 BoardButtonPressedScale = new(1.08f, 1.08f, 1.08f);

    private void Awake()
    {
        if (queue == null)
        {
            queue = GetComponent<CommandQueue>();
        }

        if (executor == null)
        {
            executor = GetComponent<ExecutionManager>();
        }

        CacheAttemptHeartDefaults();
    }

    private void Start()
    {
        EnsureBoardUi();

        if (!_hasBoardUi)
        {
            EnsureHud();
        }

        SubscribeState();
        RefreshBoardQueue();
    }

    private void OnDestroy()
    {
        UnsubscribeState();
    }

    private void Update()
    {
        if (queue == null || executor == null)
        {
            return;
        }

        if (WasPressed(KeyCode.W, KeyCode.UpArrow))
        {
            HandleAction(swapVerticalControls ? RouteControlAction.MoveDown : RouteControlAction.MoveUp);
        }
        else if (WasPressed(KeyCode.D, KeyCode.RightArrow))
        {
            HandleAction(swapHorizontalControls ? RouteControlAction.MoveLeft : RouteControlAction.MoveRight);
        }
        else if (WasPressed(KeyCode.S, KeyCode.DownArrow))
        {
            HandleAction(swapVerticalControls ? RouteControlAction.MoveUp : RouteControlAction.MoveDown);
        }
        else if (WasPressed(KeyCode.A, KeyCode.LeftArrow))
        {
            HandleAction(swapHorizontalControls ? RouteControlAction.MoveRight : RouteControlAction.MoveLeft);
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            HandleAction(RouteControlAction.Wait);
        }
        else if (Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.Backspace))
        {
            HandleAction(RouteControlAction.Undo);
        }
        else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            HandleAction(RouteControlAction.Run);
        }

        HandleBoardPointerInput();
    }

    public string GetActionKeyLabel(RouteControlAction action)
    {
        return action switch
        {
            RouteControlAction.MoveUp => swapVerticalControls ? "S / ↓" : "W / ↑",
            RouteControlAction.MoveRight => swapHorizontalControls ? "A / →" : "D / →",
            RouteControlAction.MoveDown => swapVerticalControls ? "W / ↑" : "S / ↓",
            RouteControlAction.MoveLeft => swapHorizontalControls ? "D / ←" : "A / ←",
            _ => RouteMiniGameIcons.ActionKey(action)
        };
    }

    private void EnsureHud()
    {
        if (_hasBoardUi)
        {
            return;
        }

        if (_hud == null)
        {
            _hud = FindObjectOfType<RouteMiniGameHUD>();
        }

        if (_hud == null)
        {
            GameObject hudObject = new(
                "RouteMiniGameHUD",
                typeof(RectTransform),
                typeof(Canvas),
                typeof(CanvasScaler),
                typeof(GraphicRaycaster),
                typeof(RouteMiniGameHUD));

            _hud = hudObject.GetComponent<RouteMiniGameHUD>();
        }

        if (_hud != null)
        {
            _hud.Initialize(this, queue, executor);
        }
    }

    public void HandleAction(RouteControlAction action)
    {
        _hud?.Flash(action);
        FlashBoardButton(action);

        if (queue == null || executor == null)
        {
            return;
        }

        switch (action)
        {
            case RouteControlAction.MoveUp:
                TryQueueCommand(RouteCommandType.MoveUp);
                break;

            case RouteControlAction.MoveRight:
                TryQueueCommand(RouteCommandType.MoveRight);
                break;

            case RouteControlAction.MoveDown:
                TryQueueCommand(RouteCommandType.MoveDown);
                break;

            case RouteControlAction.MoveLeft:
                TryQueueCommand(RouteCommandType.MoveLeft);
                break;

            case RouteControlAction.Wait:
                TryQueueCommand(RouteCommandType.Wait);
                break;

            case RouteControlAction.Undo:
                if (executor.IsRunning)
                {
                    executor.SetStatus("Во время прогона маршрут редактировать нельзя.", true);
                    return;
                }

                if (queue.RemoveLast(out string undoMessage))
                {
                    executor.TrimFailureMarkersToCommandCount(queue.Commands.Count);
                    executor.SetStatus(undoMessage, false);
                }
                else
                {
                    executor.SetStatus(undoMessage, true);
                }
                break;

            case RouteControlAction.Run:
                executor.TryStartRun();
                break;

            case RouteControlAction.Reset:
                bool restoreAttempts = executor.AttemptsRemaining <= 0 && !executor.IsRunning;
                executor.ResetSession(restoreAttempts, true);
                break;
        }
    }

    private void TryQueueCommand(RouteCommandType type)
    {
        if (executor.IsRunning)
        {
            executor.SetStatus("Во время прогона маршрут редактировать нельзя.", true);
            return;
        }

        if (queue.TryAddCommand(type, out string message))
        {
            executor.SetStatus(message, false);
        }
        else
        {
            executor.SetStatus(message, true);
        }
    }

    private static bool WasPressed(KeyCode primary, KeyCode secondary)
    {
        return Input.GetKeyDown(primary) || Input.GetKeyDown(secondary);
    }

    private void SubscribeState()
    {
        if (queue != null)
        {
            queue.Changed -= RefreshBoardQueue;
            queue.Changed += RefreshBoardQueue;
        }

        if (executor != null)
        {
            executor.StateChanged -= RefreshBoardQueue;
            executor.StateChanged += RefreshBoardQueue;
        }
    }

    private void UnsubscribeState()
    {
        if (queue != null)
        {
            queue.Changed -= RefreshBoardQueue;
        }

        if (executor != null)
        {
            executor.StateChanged -= RefreshBoardQueue;
        }
    }

    private void EnsureBoardUi()
    {
        _boardButtonSprites.Clear();
        _boardButtonScales.Clear();
        _boardButtonColliders.Clear();
        _boardUiButtons.Clear();
        _boardUiButtonImages.Clear();
        _boardUiButtonRects.Clear();
        _boardQueueUiImages.Clear();
        _boardQueueUiLabels.Clear();
        _boardQueueUiRoot = null;
        _boardQueueLimitText = null;
        _hasBoardUi = false;

        Canvas[] canvases = FindObjectsOfType<Canvas>(true);
        for (int index = 0; index < canvases.Length; index++)
        {
            Canvas canvas = canvases[index];
            if (canvas == null || canvas.renderMode != RenderMode.WorldSpace)
            {
                continue;
            }

            Transform buttonsRoot = FindNamedChildRecursive(canvas.transform, "Buttons");
            Transform queueBox = FindNamedChildRecursive(canvas.transform, "box");
            if (buttonsRoot == null || queueBox == null)
            {
                continue;
            }

            if (!TryBindBoardButton(buttonsRoot, "W", RouteControlAction.MoveUp) ||
                !TryBindBoardButton(buttonsRoot, "A", RouteControlAction.MoveLeft) ||
                !TryBindBoardButton(buttonsRoot, "S", RouteControlAction.MoveDown) ||
                !TryBindBoardButton(buttonsRoot, "D", RouteControlAction.MoveRight) ||
                !TryBindBoardButton(buttonsRoot, "Space", RouteControlAction.Wait) ||
                !TryBindBoardButton(buttonsRoot, "Enter", RouteControlAction.Run) ||
                !TryBindBoardButton(buttonsRoot, "R", RouteControlAction.Undo))
            {
                _boardButtonSprites.Clear();
                _boardButtonScales.Clear();
                _boardButtonColliders.Clear();
                continue;
            }

            _boardQueueBox = queueBox;
            BuildRuntimeBoardButtons(buttonsRoot);
            EnsureBoardQueueOverlay(canvas.transform as RectTransform, queueBox);

            if (_boardUiButtons.Count == 0)
            {
                EnsureBoardButtonColliders();
            }

            if (_boardQueueUiRoot == null)
            {
                EnsureBoardQueueText(queueBox);
                EnsureBoardQueueIconSlots();
            }

            _boardCamera = canvas.worldCamera != null ? canvas.worldCamera : Camera.main;
            _hasBoardUi = _boardUiButtons.Count > 0 || _boardQueueUiRoot != null || _boardQueueText != null;
            break;
        }
    }

    private bool TryBindBoardButton(Transform root, string objectName, RouteControlAction action)
    {
        Transform target = FindNamedChildRecursive(root, objectName);
        if (target == null)
        {
            return false;
        }

        SpriteRenderer renderer = target.GetComponent<SpriteRenderer>();
        if (renderer == null)
        {
            renderer = target.GetComponentInChildren<SpriteRenderer>(true);
        }

        if (renderer == null)
        {
            return false;
        }

        _boardButtonSprites[action] = renderer;
        _boardButtonScales[action] = renderer.transform.localScale;
        renderer.color = BoardButtonIdleColor;
        return true;
    }

    private void EnsureBoardButtonColliders()
    {
        foreach (KeyValuePair<RouteControlAction, SpriteRenderer> pair in _boardButtonSprites)
        {
            SpriteRenderer renderer = pair.Value;
            if (renderer == null)
            {
                continue;
            }

            BoxCollider collider = renderer.GetComponent<BoxCollider>();
            if (collider == null)
            {
                collider = renderer.gameObject.AddComponent<BoxCollider>();
            }

            if (renderer.sprite != null)
            {
                collider.center = renderer.sprite.bounds.center;
                collider.size = new Vector3(renderer.sprite.bounds.size.x * 1.15f, renderer.sprite.bounds.size.y * 1.15f, 0.08f);
            }
            else
            {
                collider.center = Vector3.zero;
                collider.size = new Vector3(1f, 1f, 0.08f);
            }

            _boardButtonColliders[collider] = pair.Key;
        }
    }

    private void EnsureBoardQueueText(Transform queueBox)
    {
        if (queueBox == null)
        {
            return;
        }

        Transform existing = queueBox.Find("QueueDisplay");
        if (existing != null)
        {
            _boardQueueText = existing.GetComponent<TextMesh>();
        }

        if (_boardQueueText == null)
        {
            GameObject queueTextObject = new("QueueDisplay", typeof(TextMesh), typeof(MeshRenderer));
            queueTextObject.transform.SetParent(queueBox, false);
            queueTextObject.layer = queueBox.gameObject.layer;
            queueTextObject.transform.localPosition = Vector3.zero;
            queueTextObject.transform.localRotation = GetBoardVisualRotation();
            queueTextObject.transform.localScale = Vector3.one * 0.09f;

            _boardQueueText = queueTextObject.GetComponent<TextMesh>();
        }
        else
        {
            _boardQueueText.transform.localRotation = GetBoardVisualRotation();
        }

        Font font = LoadRuntimeFont();
        if (font == null)
        {
            return;
        }

        _boardQueueText.font = font;
        MeshRenderer queueRenderer = _boardQueueText.GetComponent<MeshRenderer>();
        queueRenderer.sharedMaterial = font.material;
        queueRenderer.sortingOrder = 20;
        _boardQueueText.anchor = TextAnchor.MiddleCenter;
        _boardQueueText.alignment = TextAlignment.Center;
        _boardQueueText.fontSize = 64;
        _boardQueueText.characterSize = 0.11f;
        _boardQueueText.lineSpacing = 0.8f;
        _boardQueueText.color = new Color(0.96f, 0.84f, 0.55f, 1f);
        _boardQueueText.text = string.Empty;
    }

    private void RefreshBoardQueue()
    {
        if (!_hasBoardUi)
        {
            return;
        }

        RefreshAttemptHearts();

        if (queue == null)
        {
            return;
        }

        RefreshBoardQueueLimitText();

        if (_boardQueueUiRoot != null)
        {
            RefreshBoardQueueUi();
            return;
        }

        EnsureBoardQueueIconSlots();

        bool useSpriteQueue = queueArrowSprite != null || queuePauseSprite != null;
        if (useSpriteQueue)
        {
            if (_boardQueueText != null)
            {
                _boardQueueText.text = string.Empty;
            }

            RefreshBoardQueueIcons();
            return;
        }

        if (_boardQueueText == null)
        {
            return;
        }

        for (int index = 0; index < _boardQueueIconRenderers.Count; index++)
        {
            _boardQueueIconRenderers[index].enabled = false;
            _boardQueueLabelRenderers[index].gameObject.SetActive(false);
        }

        if (queue.Commands.Count == 0)
        {
            _boardQueueText.text = string.Empty;
            return;
        }

        List<string> tokens = new(queue.Commands.Count);
        for (int index = 0; index < queue.Commands.Count; index++)
        {
            tokens.Add(ToBoardQueueToken(queue.Commands[index].Type));
        }

        int firstLineCount = tokens.Count > 5 ? Mathf.CeilToInt(tokens.Count / 2f) : tokens.Count;
        string firstLine = string.Join("  ", tokens.GetRange(0, firstLineCount));
        if (tokens.Count <= firstLineCount)
        {
            _boardQueueText.text = firstLine;
            return;
        }

        string secondLine = string.Join("  ", tokens.GetRange(firstLineCount, tokens.Count - firstLineCount));
        _boardQueueText.text = $"{firstLine}\n{secondLine}";
    }

    private void EnsureBoardQueueIconSlots()
    {
        if (_boardQueueBox == null)
        {
            return;
        }

        if (_boardQueueIconsRoot == null)
        {
            Transform existing = _boardQueueBox.Find("QueueIcons");
            if (existing != null)
            {
                _boardQueueIconsRoot = existing;
            }
            else
            {
                GameObject root = new("QueueIcons");
                root.transform.SetParent(_boardQueueBox, false);
                root.transform.localPosition = Vector3.zero;
                root.transform.localRotation = GetBoardVisualRotation();
                root.transform.localScale = Vector3.one;
                root.layer = _boardQueueBox.gameObject.layer;
                _boardQueueIconsRoot = root.transform;
            }
        }

        _boardQueueIconsRoot.gameObject.layer = _boardQueueBox.gameObject.layer;
        _boardQueueIconsRoot.localRotation = GetBoardVisualRotation();

        int desiredCount = Mathf.Max(queue != null ? queue.maxCommands : 1, 1);
        while (_boardQueueIconRenderers.Count < desiredCount)
        {
            CreateBoardQueueSlot(_boardQueueIconRenderers.Count);
        }

        SyncBoardQueueVisualOrdering();
    }

    private void CreateBoardQueueSlot(int index)
    {
        GameObject slot = new($"QueueSlot_{index}");
        slot.transform.SetParent(_boardQueueIconsRoot, false);
        slot.transform.localPosition = Vector3.zero;
        slot.transform.localRotation = Quaternion.identity;
        slot.transform.localScale = Vector3.one;
        slot.layer = _boardQueueBox != null ? _boardQueueBox.gameObject.layer : 0;

        GameObject iconObject = new("Icon");
        iconObject.transform.SetParent(slot.transform, false);
        iconObject.transform.localPosition = Vector3.zero;
        iconObject.transform.localRotation = Quaternion.identity;
        iconObject.transform.localScale = Vector3.one;
        iconObject.layer = slot.layer;
        SpriteRenderer iconRenderer = iconObject.AddComponent<SpriteRenderer>();
        SpriteRenderer referenceRenderer = GetBoardVisualReferenceRenderer();
        if (referenceRenderer != null)
        {
            iconRenderer.sharedMaterial = referenceRenderer.sharedMaterial;
        }
        iconRenderer.enabled = false;
        _boardQueueIconRenderers.Add(iconRenderer);

        GameObject labelObject = new("Label", typeof(TextMesh), typeof(MeshRenderer));
        labelObject.transform.SetParent(slot.transform, false);
        labelObject.transform.localPosition = Vector3.zero;
        labelObject.transform.localRotation = Quaternion.identity;
        labelObject.transform.localScale = Vector3.one * 0.12f;
        labelObject.layer = slot.layer;
        TextMesh label = labelObject.GetComponent<TextMesh>();
        Font font = LoadRuntimeFont();
        if (font == null)
        {
            return;
        }

        label.font = font;
        MeshRenderer labelRenderer = labelObject.GetComponent<MeshRenderer>();
        labelRenderer.sharedMaterial = font.material;
        label.anchor = TextAnchor.MiddleCenter;
        label.alignment = TextAlignment.Center;
        label.fontSize = 64;
        label.characterSize = 0.12f;
        label.color = queueIconColor;
        label.text = string.Empty;
        label.gameObject.SetActive(false);
        _boardQueueLabelRenderers.Add(label);
    }

    private void SyncBoardQueueVisualOrdering()
    {
        if (_boardQueueBox == null)
        {
            return;
        }

        SpriteRenderer boxRenderer = _boardQueueBox.GetComponent<SpriteRenderer>();
        int sortingLayerId = boxRenderer != null ? boxRenderer.sortingLayerID : 0;
        int iconSortingOrder = boxRenderer != null ? boxRenderer.sortingOrder + 10 : 10;
        int labelSortingOrder = iconSortingOrder + 1;
        int textSortingOrder = iconSortingOrder + 2;

        for (int index = 0; index < _boardQueueIconRenderers.Count; index++)
        {
            SpriteRenderer iconRenderer = _boardQueueIconRenderers[index];
            if (iconRenderer != null)
            {
                iconRenderer.sortingLayerID = sortingLayerId;
                iconRenderer.sortingOrder = iconSortingOrder;
            }

            TextMesh label = _boardQueueLabelRenderers[index];
            if (label != null)
            {
                MeshRenderer labelRenderer = label.GetComponent<MeshRenderer>();
                labelRenderer.sortingLayerID = sortingLayerId;
                labelRenderer.sortingOrder = labelSortingOrder;
            }
        }

        if (_boardQueueText != null)
        {
            MeshRenderer textRenderer = _boardQueueText.GetComponent<MeshRenderer>();
            textRenderer.sortingLayerID = sortingLayerId;
            textRenderer.sortingOrder = textSortingOrder;
        }
    }

    private void RefreshBoardQueueIcons()
    {
        if (_boardQueueBox == null || _boardQueueIconsRoot == null)
        {
            return;
        }

        SpriteRenderer boxRenderer = _boardQueueBox.GetComponent<SpriteRenderer>();
        if (boxRenderer == null || boxRenderer.sprite == null)
        {
            return;
        }

        int commandCount = queue.Commands.Count;
        int slotCount = _boardQueueIconRenderers.Count;
        if (slotCount == 0)
        {
            return;
        }

        Vector2 boxSize = boxRenderer.sprite.bounds.size;
        int iconsPerRow = Mathf.Max(1, queueIconsPerRow);
        int rowCount = Mathf.Max(1, Mathf.CeilToInt(Mathf.Max(commandCount, 1) / (float)iconsPerRow));
        float contentWidth = boxSize.x * queueContentWidthFactor;
        float contentHeight = boxSize.y * queueContentHeightFactor;
        float columnWidth = contentWidth / iconsPerRow;
        float rowHeight = contentHeight / rowCount;
        float horizontalSign = GetBoardQueueHorizontalSign();

        for (int index = 0; index < slotCount; index++)
        {
            SpriteRenderer iconRenderer = _boardQueueIconRenderers[index];
            TextMesh labelRenderer = _boardQueueLabelRenderers[index];

            if (index >= commandCount)
            {
                iconRenderer.enabled = false;
                labelRenderer.gameObject.SetActive(false);
                continue;
            }

            int row = index / iconsPerRow;
            int column = index % iconsPerRow;
            float x = (-contentWidth * 0.5f + (columnWidth * 0.5f) + (column * columnWidth)) * horizontalSign;
            float y = contentHeight * 0.5f - (rowHeight * 0.5f) - (row * rowHeight);
            Vector3 localPosition = new(x, y, 0f);

            iconRenderer.transform.parent.localPosition = localPosition;
            bool isFailedCommand = IsFailedQueueCommand(index);
            iconRenderer.color = isFailedCommand ? queueFailureColor : Color.white;
            labelRenderer.color = isFailedCommand ? queueFailureColor : queueIconColor;

            RouteCommandType commandType = queue.Commands[index].Type;
            if (TryConfigureBoardQueueIcon(commandType, iconRenderer))
            {
                labelRenderer.gameObject.SetActive(false);
                continue;
            }

            iconRenderer.enabled = false;
            labelRenderer.text = ToBoardQueueToken(commandType);
            labelRenderer.gameObject.SetActive(true);
        }
    }

    private bool TryConfigureBoardQueueIcon(RouteCommandType commandType, SpriteRenderer iconRenderer)
    {
        Sprite sprite = null;
        float zRotation = GetBoardQueueIconRotation(commandType);

        switch (commandType)
        {
            case RouteCommandType.MoveUp:
                sprite = queueArrowSprite;
                break;

            case RouteCommandType.MoveRight:
                sprite = queueArrowSprite;
                break;

            case RouteCommandType.MoveDown:
                sprite = queueArrowSprite;
                break;

            case RouteCommandType.MoveLeft:
                sprite = queueArrowSprite;
                break;

            case RouteCommandType.Wait:
                sprite = queuePauseSprite;
                zRotation = 0f;
                break;
        }

        if (sprite == null)
        {
            return false;
        }

        iconRenderer.sprite = sprite;
        iconRenderer.enabled = true;
        iconRenderer.transform.localRotation = Quaternion.Euler(0f, 0f, zRotation);

        Vector2 spriteSize = sprite.bounds.size;
        float maxSpriteSide = Mathf.Max(Mathf.Max(spriteSize.x, spriteSize.y), 0.0001f);
        float desiredSize = Mathf.Min(
            _boardQueueBox.GetComponent<SpriteRenderer>().sprite.bounds.size.x * queueContentWidthFactor / Mathf.Max(1, queueIconsPerRow),
            _boardQueueBox.GetComponent<SpriteRenderer>().sprite.bounds.size.y * queueContentHeightFactor / 2f) * 0.95f;

        float uniformScale = desiredSize / maxSpriteSide;
        iconRenderer.transform.localScale = Vector3.one * uniformScale;
        return true;
    }

    private void HandleBoardPointerInput()
    {
        if (!_hasBoardUi || _boardUiButtons.Count > 0 || !Input.GetMouseButtonDown(0))
        {
            return;
        }

        if (_boardCamera == null)
        {
            _boardCamera = Camera.main;
        }

        if (_boardCamera == null)
        {
            return;
        }

        Ray ray = _boardCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity, ~0, QueryTriggerInteraction.Collide);
        if (hits.Length > 0)
        {
            Array.Sort(hits, (left, right) => left.distance.CompareTo(right.distance));

            for (int index = 0; index < hits.Length; index++)
            {
                if (_boardButtonColliders.TryGetValue(hits[index].collider, out RouteControlAction action))
                {
                    HandleAction(action);
                    return;
                }
            }
        }

        if (TryResolveBoardSpriteHit(ray, out RouteControlAction spriteAction))
        {
            HandleAction(spriteAction);
        }
    }

    private void FlashBoardButton(RouteControlAction action)
    {
        if (_boardUiButtonImages.TryGetValue(action, out Image image) &&
            image != null &&
            _boardUiButtonRects.TryGetValue(action, out RectTransform buttonRect) &&
            buttonRect != null)
        {
            if (_boardButtonFlashRoutines.TryGetValue(action, out Coroutine runningUiRoutine) && runningUiRoutine != null)
            {
                StopCoroutine(runningUiRoutine);
            }

            _boardButtonFlashRoutines[action] = StartCoroutine(FlashBoardUiButtonRoutine(action, image, buttonRect));
            return;
        }

        if (!_boardButtonSprites.TryGetValue(action, out SpriteRenderer renderer) || renderer == null)
        {
            return;
        }

        if (_boardButtonFlashRoutines.TryGetValue(action, out Coroutine runningRoutine) && runningRoutine != null)
        {
            StopCoroutine(runningRoutine);
        }

        _boardButtonFlashRoutines[action] = StartCoroutine(FlashBoardButtonRoutine(action, renderer));
    }

    private IEnumerator FlashBoardButtonRoutine(RouteControlAction action, SpriteRenderer renderer)
    {
        if (!_boardButtonScales.TryGetValue(action, out Vector3 baseScale))
        {
            baseScale = renderer.transform.localScale;
        }

        renderer.color = BoardButtonActiveColor;
        renderer.transform.localScale = Vector3.Scale(baseScale, BoardButtonPressedScale);
        yield return new WaitForSeconds(0.12f);

        if (renderer != null)
        {
            renderer.color = BoardButtonIdleColor;
            renderer.transform.localScale = baseScale;
        }

        _boardButtonFlashRoutines[action] = null;
    }

    private IEnumerator FlashBoardUiButtonRoutine(RouteControlAction action, Image image, RectTransform buttonRect)
    {
        if (!_boardButtonScales.TryGetValue(action, out Vector3 baseScale))
        {
            baseScale = buttonRect.localScale;
        }

        image.color = BoardButtonActiveColor;
        buttonRect.localScale = Vector3.Scale(baseScale, BoardButtonPressedScale);
        yield return new WaitForSeconds(0.12f);

        if (image != null)
        {
            image.color = BoardButtonIdleColor;
        }

        if (buttonRect != null)
        {
            buttonRect.localScale = baseScale;
        }

        _boardButtonFlashRoutines[action] = null;
    }

    private static string ToBoardQueueToken(RouteCommandType type)
    {
        return type switch
        {
            RouteCommandType.MoveUp => "W",
            RouteCommandType.MoveLeft => "A",
            RouteCommandType.MoveDown => "S",
            RouteCommandType.MoveRight => "D",
            RouteCommandType.Wait => "SP",
            _ => string.Empty
        };
    }

    private bool TryResolveBoardSpriteHit(Ray ray, out RouteControlAction action)
    {
        action = default;
        float closestDistance = float.PositiveInfinity;
        bool hasHit = false;

        foreach (KeyValuePair<RouteControlAction, SpriteRenderer> pair in _boardButtonSprites)
        {
            SpriteRenderer renderer = pair.Value;
            if (renderer == null)
            {
                continue;
            }

            if (!renderer.bounds.IntersectRay(ray, out float distance))
            {
                continue;
            }

            if (distance >= closestDistance)
            {
                continue;
            }

            closestDistance = distance;
            action = pair.Key;
            hasHit = true;
        }

        return hasHit;
    }

    private Quaternion GetBoardVisualRotation()
    {
        SpriteRenderer referenceRenderer = GetBoardVisualReferenceRenderer();
        if (referenceRenderer == null)
        {
            return Quaternion.Euler(0f, 180f, 0f);
        }

        return referenceRenderer.transform.localRotation;
    }

    private SpriteRenderer GetBoardVisualReferenceRenderer()
    {
        if (_boardButtonSprites.TryGetValue(RouteControlAction.MoveUp, out SpriteRenderer upRenderer) && upRenderer != null)
        {
            return upRenderer;
        }

        foreach (SpriteRenderer renderer in _boardButtonSprites.Values)
        {
            if (renderer != null)
            {
                return renderer;
            }
        }

        return null;
    }

    private void BuildRuntimeBoardButtons(Transform buttonsRoot)
    {
        RectTransform buttonParent = buttonsRoot as RectTransform;
        if (buttonParent == null)
        {
            return;
        }

        CreateRuntimeBoardButton(buttonParent, RouteControlAction.MoveUp, "RuntimeButton_W");
        CreateRuntimeBoardButton(buttonParent, RouteControlAction.MoveLeft, "RuntimeButton_A");
        CreateRuntimeBoardButton(buttonParent, RouteControlAction.MoveDown, "RuntimeButton_S");
        CreateRuntimeBoardButton(buttonParent, RouteControlAction.MoveRight, "RuntimeButton_D");
        CreateRuntimeBoardButton(buttonParent, RouteControlAction.Wait, "RuntimeButton_Space");
        CreateRuntimeBoardButton(buttonParent, RouteControlAction.Run, "RuntimeButton_Enter");
        CreateRuntimeBoardButton(buttonParent, RouteControlAction.Undo, "RuntimeButton_R");
    }

    private void CreateRuntimeBoardButton(RectTransform parent, RouteControlAction action, string objectName)
    {
        if (!_boardButtonSprites.TryGetValue(action, out SpriteRenderer sourceRenderer) ||
            sourceRenderer == null ||
            sourceRenderer.sprite == null)
        {
            return;
        }

        RectTransform buttonRect = parent.Find(objectName) as RectTransform;
        if (buttonRect == null)
        {
            GameObject buttonObject = new(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            buttonRect = buttonObject.GetComponent<RectTransform>();
            buttonRect.SetParent(parent, false);
        }

        buttonRect.localPosition = sourceRenderer.transform.localPosition;
        buttonRect.localRotation = sourceRenderer.transform.localRotation;
        buttonRect.localScale = sourceRenderer.transform.localScale;
        buttonRect.sizeDelta = sourceRenderer.sprite.bounds.size;
        buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
        buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
        buttonRect.pivot = new Vector2(0.5f, 0.5f);
        buttonRect.SetAsLastSibling();

        Image image = buttonRect.GetComponent<Image>();
        image.sprite = sourceRenderer.sprite;
        image.color = sourceRenderer.color;
        image.type = Image.Type.Simple;
        image.preserveAspect = true;
        image.raycastTarget = true;

        Button button = buttonRect.GetComponent<Button>();
        button.transition = Selectable.Transition.None;
        button.targetGraphic = image;
        button.onClick.RemoveAllListeners();
        RouteControlAction capturedAction = action;
        button.onClick.AddListener(() => HandleAction(capturedAction));

        _boardUiButtons[action] = button;
        _boardUiButtonImages[action] = image;
        _boardUiButtonRects[action] = buttonRect;
        _boardButtonScales[action] = buttonRect.localScale;

        sourceRenderer.enabled = false;
    }

    private void EnsureBoardQueueOverlay(RectTransform canvasRect, Transform queueBox)
    {
        if (canvasRect == null || queueBox == null)
        {
            return;
        }

        SpriteRenderer boxRenderer = queueBox.GetComponent<SpriteRenderer>();
        if (boxRenderer == null || boxRenderer.sprite == null)
        {
            return;
        }

        RectTransform overlay = canvasRect.Find("RuntimeQueueOverlay") as RectTransform;
        if (overlay == null)
        {
            GameObject overlayObject = new("RuntimeQueueOverlay", typeof(RectTransform));
            overlay = overlayObject.GetComponent<RectTransform>();
            overlay.SetParent(canvasRect, false);
        }

        overlay.localPosition = queueBox.localPosition;
        overlay.localRotation = queueBox.localRotation;
        overlay.localScale = queueBox.localScale;
        overlay.sizeDelta = boxRenderer.sprite.bounds.size;
        overlay.anchorMin = new Vector2(0.5f, 0.5f);
        overlay.anchorMax = new Vector2(0.5f, 0.5f);
        overlay.pivot = new Vector2(0.5f, 0.5f);
        overlay.SetAsLastSibling();
        _boardQueueUiRoot = overlay;

        EnsureBoardQueueLimitText();
        EnsureBoardQueueUiSlots();
    }

    private void EnsureBoardQueueLimitText()
    {
        if (_boardQueueUiRoot == null || HasExternalQueueLimitTarget())
        {
            return;
        }

        RectTransform limitRect = _boardQueueUiRoot.Find("QueueLimitText") as RectTransform;
        if (limitRect == null)
        {
            GameObject limitObject = new("QueueLimitText", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            limitRect = limitObject.GetComponent<RectTransform>();
            limitRect.SetParent(_boardQueueUiRoot, false);
        }

        limitRect.anchorMin = new Vector2(1f, 1f);
        limitRect.anchorMax = new Vector2(1f, 1f);
        limitRect.pivot = new Vector2(1f, 1f);
        limitRect.sizeDelta = new Vector2(132f, 42f);
        limitRect.anchoredPosition = new Vector2(-10f, -8f);
        limitRect.localScale = Vector3.one;
        limitRect.localRotation = Quaternion.identity;
        limitRect.SetAsLastSibling();

        _boardQueueLimitText = limitRect.GetComponent<Text>();
        Font font = LoadRuntimeFont();
        if (font != null)
        {
            _boardQueueLimitText.font = font;
        }

        _boardQueueLimitText.alignment = TextAnchor.UpperRight;
        _boardQueueLimitText.fontStyle = FontStyle.Bold;
        _boardQueueLimitText.fontSize = 28;
        _boardQueueLimitText.resizeTextForBestFit = true;
        _boardQueueLimitText.resizeTextMinSize = 12;
        _boardQueueLimitText.resizeTextMaxSize = 32;
        _boardQueueLimitText.horizontalOverflow = HorizontalWrapMode.Overflow;
        _boardQueueLimitText.verticalOverflow = VerticalWrapMode.Overflow;
        _boardQueueLimitText.raycastTarget = false;
        _boardQueueLimitText.text = string.Empty;
    }

    private void EnsureBoardQueueUiSlots()
    {
        if (_boardQueueUiRoot == null)
        {
            return;
        }

        int desiredCount = Mathf.Max(queue != null ? queue.maxCommands : 1, 1);
        while (_boardQueueUiImages.Count < desiredCount)
        {
            CreateBoardQueueUiSlot(_boardQueueUiImages.Count);
        }
    }

    private void CreateBoardQueueUiSlot(int index)
    {
        GameObject slotObject = new($"QueueUiSlot_{index}", typeof(RectTransform));
        RectTransform slotRect = slotObject.GetComponent<RectTransform>();
        slotRect.SetParent(_boardQueueUiRoot, false);
        slotRect.anchorMin = new Vector2(0.5f, 0.5f);
        slotRect.anchorMax = new Vector2(0.5f, 0.5f);
        slotRect.pivot = new Vector2(0.5f, 0.5f);
        slotRect.localScale = Vector3.one;
        slotRect.localRotation = Quaternion.identity;

        GameObject iconObject = new("Icon", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        RectTransform iconRect = iconObject.GetComponent<RectTransform>();
        iconRect.SetParent(slotRect, false);
        iconRect.anchorMin = Vector2.zero;
        iconRect.anchorMax = Vector2.one;
        iconRect.offsetMin = Vector2.zero;
        iconRect.offsetMax = Vector2.zero;
        iconRect.localRotation = Quaternion.identity;
        Image iconImage = iconObject.GetComponent<Image>();
        iconImage.preserveAspect = true;
        iconImage.raycastTarget = false;
        iconImage.enabled = false;
        _boardQueueUiImages.Add(iconImage);

        GameObject labelObject = new("Label", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        RectTransform labelRect = labelObject.GetComponent<RectTransform>();
        labelRect.SetParent(slotRect, false);
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;
        labelRect.localRotation = Quaternion.identity;
        Text label = labelObject.GetComponent<Text>();
        Font font = LoadRuntimeFont();
        if (font != null)
        {
            label.font = font;
        }

        label.alignment = TextAnchor.MiddleCenter;
        label.color = queueIconColor;
        label.resizeTextForBestFit = true;
        label.resizeTextMinSize = 10;
        label.resizeTextMaxSize = 80;
        label.raycastTarget = false;
        label.enabled = false;
        _boardQueueUiLabels.Add(label);
    }

    private void RefreshBoardQueueUi()
    {
        if (_boardQueueUiRoot == null)
        {
            return;
        }

        EnsureBoardQueueUiSlots();

        int commandCount = queue.Commands.Count;
        int slotCount = _boardQueueUiImages.Count;
        Vector2 boxSize = _boardQueueUiRoot.rect.size;
        int iconsPerRow = Mathf.Max(1, queueIconsPerRow);
        int rowCount = Mathf.Max(1, Mathf.CeilToInt(Mathf.Max(commandCount, 1) / (float)iconsPerRow));
        float contentWidth = boxSize.x * queueContentWidthFactor;
        float contentHeight = boxSize.y * queueContentHeightFactor;
        float columnWidth = contentWidth / iconsPerRow;
        float rowHeight = contentHeight / rowCount;
        float desiredSize = Mathf.Min(columnWidth, rowHeight) * 0.82f;
        float horizontalSign = GetBoardQueueHorizontalSign();

        for (int index = 0; index < slotCount; index++)
        {
            Image icon = _boardQueueUiImages[index];
            Text label = _boardQueueUiLabels[index];
            RectTransform slotRect = icon.rectTransform.parent as RectTransform;

            if (index >= commandCount || slotRect == null)
            {
                icon.enabled = false;
                label.enabled = false;
                continue;
            }

            int row = index / iconsPerRow;
            int column = index % iconsPerRow;
            float x = (-contentWidth * 0.5f + (columnWidth * 0.5f) + (column * columnWidth)) * horizontalSign;
            float y = contentHeight * 0.5f - (rowHeight * 0.5f) - (row * rowHeight);

            slotRect.anchoredPosition = new Vector2(x, y);
            slotRect.sizeDelta = Vector2.one * desiredSize;
            bool isFailedCommand = IsFailedQueueCommand(index);

            RouteCommandType commandType = queue.Commands[index].Type;
            if (TryConfigureBoardQueueUiIcon(commandType, icon))
            {
                icon.color = isFailedCommand ? queueFailureColor : Color.white;
                label.enabled = false;
                continue;
            }

            icon.enabled = false;
            label.text = ToBoardQueueToken(commandType);
            label.color = isFailedCommand ? queueFailureColor : queueIconColor;
            label.enabled = true;
        }
    }

    private void RefreshBoardQueueLimitText()
    {
        if (queue == null)
        {
            return;
        }

        int maxCommands = Mathf.Max(queue.maxCommands, 1);
        int currentCommands = queue.Commands.Count;
        string limitText = $"{currentCommands}/{maxCommands}";
        Color limitColor = currentCommands >= maxCommands ? queueFailureColor : queueIconColor;

        if (queueLimitTextOverride != null)
        {
            queueLimitTextOverride.text = limitText;
            queueLimitTextOverride.color = limitColor;
        }

        if (queueLimitTextMeshOverride != null)
        {
            queueLimitTextMeshOverride.text = limitText;
            queueLimitTextMeshOverride.color = limitColor;
        }

        if (HasExternalQueueLimitTarget())
        {
            return;
        }

        if (_boardQueueUiRoot == null)
        {
            return;
        }

        EnsureBoardQueueLimitText();
        if (_boardQueueLimitText == null)
        {
            return;
        }

        _boardQueueLimitText.text = limitText;
        _boardQueueLimitText.color = limitColor;
    }

    private bool HasExternalQueueLimitTarget()
    {
        return queueLimitTextOverride != null || queueLimitTextMeshOverride != null;
    }

    private void CacheAttemptHeartDefaults()
    {
        _attemptHeartImageDefaults = new Sprite[attemptHeartImages != null ? attemptHeartImages.Length : 0];
        for (int index = 0; index < _attemptHeartImageDefaults.Length; index++)
        {
            _attemptHeartImageDefaults[index] = attemptHeartImages[index] != null ? attemptHeartImages[index].sprite : null;
        }

        _attemptHeartRendererDefaults = new Sprite[attemptHeartSpriteRenderers != null ? attemptHeartSpriteRenderers.Length : 0];
        for (int index = 0; index < _attemptHeartRendererDefaults.Length; index++)
        {
            _attemptHeartRendererDefaults[index] = attemptHeartSpriteRenderers[index] != null ? attemptHeartSpriteRenderers[index].sprite : null;
        }
    }

    private void RefreshAttemptHearts()
    {
        if (executor == null)
        {
            return;
        }

        int remainingAttempts = Mathf.Max(0, executor.AttemptsRemaining);
        int maxAttempts = Mathf.Max(1, executor.MaxAttempts);

        RefreshAttemptHeartImages(remainingAttempts, maxAttempts);
        RefreshAttemptHeartRenderers(remainingAttempts, maxAttempts);
    }

    private void RefreshAttemptHeartImages(int remainingAttempts, int maxAttempts)
    {
        if (attemptHeartImages == null || attemptHeartImages.Length == 0)
        {
            return;
        }

        for (int index = 0; index < attemptHeartImages.Length; index++)
        {
            Image targetImage = attemptHeartImages[index];
            if (targetImage == null)
            {
                continue;
            }

            bool isUsedSlot = index < maxAttempts;
            targetImage.enabled = isUsedSlot;
            if (!isUsedSlot)
            {
                continue;
            }

            bool isAvailable = index < remainingAttempts;
            Sprite filledSprite = attemptHeartFilledSprite != null ? attemptHeartFilledSprite : GetCachedHeartSprite(_attemptHeartImageDefaults, index, targetImage.sprite);
            Sprite spentSprite = attemptHeartSpentSprite != null ? attemptHeartSpentSprite : filledSprite;
            targetImage.sprite = isAvailable ? filledSprite : spentSprite;
            targetImage.color = Color.white;
        }
    }

    private void RefreshAttemptHeartRenderers(int remainingAttempts, int maxAttempts)
    {
        if (attemptHeartSpriteRenderers == null || attemptHeartSpriteRenderers.Length == 0)
        {
            return;
        }

        for (int index = 0; index < attemptHeartSpriteRenderers.Length; index++)
        {
            SpriteRenderer targetRenderer = attemptHeartSpriteRenderers[index];
            if (targetRenderer == null)
            {
                continue;
            }

            bool isUsedSlot = index < maxAttempts;
            targetRenderer.enabled = isUsedSlot;
            if (!isUsedSlot)
            {
                continue;
            }

            bool isAvailable = index < remainingAttempts;
            Sprite filledSprite = attemptHeartFilledSprite != null ? attemptHeartFilledSprite : GetCachedHeartSprite(_attemptHeartRendererDefaults, index, targetRenderer.sprite);
            Sprite spentSprite = attemptHeartSpentSprite != null ? attemptHeartSpentSprite : filledSprite;
            targetRenderer.sprite = isAvailable ? filledSprite : spentSprite;
            targetRenderer.color = Color.white;
        }
    }

    private static Sprite GetCachedHeartSprite(Sprite[] cache, int index, Sprite fallback)
    {
        if (cache != null && index >= 0 && index < cache.Length && cache[index] != null)
        {
            return cache[index];
        }

        return fallback;
    }

    private bool TryConfigureBoardQueueUiIcon(RouteCommandType commandType, Image icon)
    {
        Sprite sprite = null;
        float zRotation = GetBoardQueueIconRotation(commandType);

        switch (commandType)
        {
            case RouteCommandType.MoveUp:
                sprite = queueArrowSprite;
                break;

            case RouteCommandType.MoveRight:
                sprite = queueArrowSprite;
                break;

            case RouteCommandType.MoveDown:
                sprite = queueArrowSprite;
                break;

            case RouteCommandType.MoveLeft:
                sprite = queueArrowSprite;
                break;

            case RouteCommandType.Wait:
                sprite = queuePauseSprite;
                zRotation = 0f;
                break;
        }

        if (sprite == null)
        {
            return false;
        }

        icon.sprite = sprite;
        icon.rectTransform.localRotation = Quaternion.Euler(0f, 0f, zRotation);
        icon.enabled = true;
        return true;
    }

    private float GetBoardQueueIconRotation(RouteCommandType commandType)
    {
        float horizontalSign = GetBoardQueueHorizontalSign();
        float verticalSign = GetBoardQueueVerticalSign();

        return commandType switch
        {
            RouteCommandType.MoveUp => queueArrowBaseRotation + (verticalSign < 0f ? 180f : 0f),
            RouteCommandType.MoveRight => queueArrowBaseRotation + (horizontalSign < 0f ? 90f : -90f),
            RouteCommandType.MoveDown => queueArrowBaseRotation + (verticalSign < 0f ? 0f : 180f),
            RouteCommandType.MoveLeft => queueArrowBaseRotation + (horizontalSign < 0f ? -90f : 90f),
            _ => 0f
        };
    }

    private float GetBoardQueueHorizontalSign()
    {
        if (_boardButtonSprites.TryGetValue(RouteControlAction.MoveLeft, out SpriteRenderer leftRenderer) &&
            _boardButtonSprites.TryGetValue(RouteControlAction.MoveRight, out SpriteRenderer rightRenderer) &&
            leftRenderer != null &&
            rightRenderer != null)
        {
            return leftRenderer.transform.localPosition.x > rightRenderer.transform.localPosition.x ? -1f : 1f;
        }

        return 1f;
    }

    private float GetBoardQueueVerticalSign()
    {
        if (_boardButtonSprites.TryGetValue(RouteControlAction.MoveUp, out SpriteRenderer upRenderer) &&
            _boardButtonSprites.TryGetValue(RouteControlAction.MoveDown, out SpriteRenderer downRenderer) &&
            upRenderer != null &&
            downRenderer != null)
        {
            return upRenderer.transform.localPosition.y < downRenderer.transform.localPosition.y ? -1f : 1f;
        }

        return 1f;
    }

    private bool IsFailedQueueCommand(int commandIndex)
    {
        return executor != null &&
               executor.LastFailedCommandIndex >= 0 &&
               commandIndex >= executor.LastFailedCommandIndex;
    }

    private static Font LoadRuntimeFont()
    {
        try
        {
            Font legacyFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (legacyFont != null)
            {
                return legacyFont;
            }
        }
        catch (Exception)
        {
        }

        try
        {
            return Resources.GetBuiltinResource<Font>("Arial.ttf");
        }
        catch (Exception)
        {
            return null;
        }
    }

    private static Transform FindNamedChildRecursive(Transform root, string objectName)
    {
        if (root == null)
        {
            return null;
        }

        if (root.name == objectName)
        {
            return root;
        }

        for (int index = 0; index < root.childCount; index++)
        {
            Transform result = FindNamedChildRecursive(root.GetChild(index), objectName);
            if (result != null)
            {
                return result;
            }
        }

        return null;
    }
}
