using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game
{
    public class RouteMiniGameHUD : MonoBehaviour
    {
        private readonly Dictionary<RouteControlAction, Image> _buttonImages = new();
        private readonly List<Text> _queueSlotTexts = new();

        private MiniGameInputHandler _inputHandler;
        private CommandQueue _queue;
        private ExecutionManager _executor;

        private Text _attemptsText;
        private Text _limitsText;
        private Text _statusText;
        private Text _remainingArgumentsText;

        private readonly Color _buttonIdleColor = new(0.17f, 0.19f, 0.24f, 0.95f);
        private readonly Color _buttonActiveColor = new(0.85f, 0.67f, 0.2f, 1f);
        private readonly Color _slotIdleColor = new(0.14f, 0.15f, 0.19f, 0.9f);
        private readonly Color _slotFilledColor = new(0.23f, 0.29f, 0.38f, 0.95f);
        private readonly Color _slotFailedColor = new(0.72f, 0.23f, 0.23f, 0.95f);

        public void Initialize(MiniGameInputHandler inputHandler, CommandQueue queue, ExecutionManager executor)
        {
            _inputHandler = inputHandler;
            _queue = queue;
            _executor = executor;

            EnsureEventSystem();
            BuildCanvas();

            if (_queue != null)
            {
                _queue.Changed -= Refresh;
                _queue.Changed += Refresh;
            }

            if (_executor != null)
            {
                _executor.StateChanged -= Refresh;
                _executor.StateChanged += Refresh;
            }

            Refresh();
        }

        public void Flash(RouteControlAction action)
        {
            if (_buttonImages.TryGetValue(action, out Image image) && image != null)
            {
                StartCoroutine(FlashRoutine(image));
            }
        }

        private void OnDestroy()
        {
            if (_queue != null)
            {
                _queue.Changed -= Refresh;
            }

            if (_executor != null)
            {
                _executor.StateChanged -= Refresh;
            }
        }

        private void BuildCanvas()
        {
            _buttonImages.Clear();
            _queueSlotTexts.Clear();

            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            Canvas canvas = gameObject.GetComponent<Canvas>();
            if (canvas == null)
            {
                canvas = gameObject.AddComponent<Canvas>();
            }

            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;

            CanvasScaler scaler = gameObject.GetComponent<CanvasScaler>();
            if (scaler == null)
            {
                scaler = gameObject.AddComponent<CanvasScaler>();
            }

            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.85f;

            if (gameObject.GetComponent<GraphicRaycaster>() == null)
            {
                gameObject.AddComponent<GraphicRaycaster>();
            }

            RectTransform root = CreatePanel("Root", transform, new Color(0f, 0f, 0f, 0f));
            root.anchorMin = new Vector2(1f, 0f);
            root.anchorMax = new Vector2(1f, 1f);
            root.pivot = new Vector2(1f, 0.5f);
            root.sizeDelta = new Vector2(360f, -40f);
            root.anchoredPosition = new Vector2(-20f, 0f);

            VerticalLayoutGroup rootLayout = root.gameObject.AddComponent<VerticalLayoutGroup>();
            rootLayout.padding = new RectOffset(18, 18, 18, 18);
            rootLayout.spacing = 12f;
            rootLayout.childControlHeight = false;
            rootLayout.childControlWidth = true;
            rootLayout.childForceExpandHeight = false;
            rootLayout.childForceExpandWidth = true;

            CreateHeader(root);
            CreateQueueSection(root);
            CreateButtonsSection(root);
            CreateFooter(root);
        }

        private void CreateHeader(RectTransform parent)
        {
            RectTransform panel = CreatePanel("Header", parent, new Color(0.08f, 0.09f, 0.12f, 0.9f));
            LayoutElement headerLayout = panel.gameObject.AddComponent<LayoutElement>();
            headerLayout.preferredHeight = 150f;

            VerticalLayoutGroup layout = panel.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(16, 16, 14, 14);
            layout.spacing = 8f;
            layout.childControlHeight = false;
            layout.childControlWidth = true;
            layout.childForceExpandHeight = false;

            CreateText(panel, "Title", "Маршрут доводов", 28, FontStyle.Bold, TextAnchor.MiddleLeft);
            _attemptsText = CreateText(panel, "Attempts", string.Empty, 19, FontStyle.Bold, TextAnchor.MiddleLeft);
            _remainingArgumentsText = CreateText(panel, "Remaining", string.Empty, 18, FontStyle.Normal, TextAnchor.MiddleLeft);
            _limitsText = CreateText(panel, "Limits", string.Empty, 16, FontStyle.Normal, TextAnchor.UpperLeft);
        }

        private void CreateQueueSection(RectTransform parent)
        {
            RectTransform panel = CreatePanel("Queue", parent, new Color(0.08f, 0.09f, 0.12f, 0.92f));
            LayoutElement queueLayout = panel.gameObject.AddComponent<LayoutElement>();
            queueLayout.preferredHeight = 420f;

            VerticalLayoutGroup layout = panel.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(16, 16, 14, 14);
            layout.spacing = 8f;
            layout.childControlHeight = false;
            layout.childControlWidth = true;
            layout.childForceExpandHeight = false;

            CreateText(panel, "QueueTitle", "Очередь команд", 22, FontStyle.Bold, TextAnchor.MiddleLeft);

            int slotCount = Mathf.Max(_queue != null ? _queue.maxCommands : 7, 1);
            for (int index = 0; index < slotCount; index++)
            {
                RectTransform slot = CreatePanel($"Slot_{index}", panel, _slotIdleColor);
                LayoutElement slotLayout = slot.gameObject.AddComponent<LayoutElement>();
                slotLayout.preferredHeight = 38f;

                Text slotText = CreateText(slot, $"SlotText_{index}", $"{index + 1}. ·", 18, FontStyle.Normal, TextAnchor.MiddleLeft);
                _queueSlotTexts.Add(slotText);
            }
        }

        private void CreateButtonsSection(RectTransform parent)
        {
            RectTransform panel = CreatePanel("Controls", parent, new Color(0.08f, 0.09f, 0.12f, 0.92f));
            LayoutElement controlsLayout = panel.gameObject.AddComponent<LayoutElement>();
            controlsLayout.preferredHeight = 330f;

            VerticalLayoutGroup layout = panel.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(16, 16, 14, 14);
            layout.spacing = 10f;
            layout.childControlHeight = false;
            layout.childControlWidth = true;
            layout.childForceExpandHeight = false;

            CreateText(panel, "ControlsTitle", "Управление", 22, FontStyle.Bold, TextAnchor.MiddleLeft);

            RectTransform grid = CreatePanel("ButtonGrid", panel, new Color(0f, 0f, 0f, 0f));
            GridLayoutGroup gridLayout = grid.gameObject.AddComponent<GridLayoutGroup>();
            gridLayout.cellSize = new Vector2(150f, 64f);
            gridLayout.spacing = new Vector2(8f, 8f);
            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = 2;

            AddButton(grid, RouteControlAction.MoveUp);
            AddButton(grid, RouteControlAction.MoveRight);
            AddButton(grid, RouteControlAction.MoveDown);
            AddButton(grid, RouteControlAction.MoveLeft);
            AddButton(grid, RouteControlAction.Wait);
            AddButton(grid, RouteControlAction.Undo);
            AddButton(grid, RouteControlAction.Run);

            RectTransform resetRow = CreatePanel("ResetRow", panel, new Color(0f, 0f, 0f, 0f));
            LayoutElement resetLayout = resetRow.gameObject.AddComponent<LayoutElement>();
            resetLayout.preferredHeight = 64f;
            AddButton(resetRow, RouteControlAction.Reset, true);
        }

        private void CreateFooter(RectTransform parent)
        {
            RectTransform panel = CreatePanel("Footer", parent, new Color(0.08f, 0.09f, 0.12f, 0.92f));
            LayoutElement footerLayout = panel.gameObject.AddComponent<LayoutElement>();
            footerLayout.preferredHeight = 120f;

            VerticalLayoutGroup layout = panel.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(16, 16, 14, 14);
            layout.spacing = 6f;
            layout.childControlHeight = false;
            layout.childControlWidth = true;
            layout.childForceExpandHeight = false;

            _statusText = CreateText(panel, "Status", string.Empty, 17, FontStyle.Bold, TextAnchor.UpperLeft);
            CreateText(panel, "Hint", "W/A/S/D или стрелки - шаг по направлению, Space - пауза, R - отмена, Enter - запуск.", 14, FontStyle.Normal, TextAnchor.UpperLeft);
        }

        private void AddButton(RectTransform parent, RouteControlAction action, bool stretch = false)
        {
            RectTransform buttonRoot = CreatePanel($"{action}_Button", parent, _buttonIdleColor);
            if (stretch)
            {
                buttonRoot.anchorMin = new Vector2(0f, 0f);
                buttonRoot.anchorMax = new Vector2(1f, 1f);
                buttonRoot.offsetMin = Vector2.zero;
                buttonRoot.offsetMax = Vector2.zero;
            }

            Image image = buttonRoot.GetComponent<Image>();
            Button button = buttonRoot.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(() => _inputHandler?.HandleAction(action));

            _buttonImages[action] = image;

            CreateText(buttonRoot, $"{action}_Text",
                $"{RouteMiniGameIcons.Action(action)}  {RouteMiniGameIcons.ActionLabel(action)}\n{GetActionKeyLabel(action)}",
                16,
                FontStyle.Bold,
                TextAnchor.MiddleCenter);
        }

        private void Refresh()
        {
            if (_queue == null || _executor == null)
            {
                return;
            }

            int desiredSlotCount = Mathf.Max(_queue.maxCommands, 1);
            if (_queueSlotTexts.Count != desiredSlotCount)
            {
                BuildCanvas();
            }

            _attemptsText.text = $"Попытки: {_executor.AttemptsRemaining}/{_executor.MaxAttempts}";
            _remainingArgumentsText.text = _executor.Grid != null
                ? $"Осталось доводов: {_executor.Grid.RemainingArguments}"
                : "Осталось доводов: -";
            _limitsText.text = _queue.GetLimitSummary();
            _statusText.text = string.IsNullOrEmpty(_executor.StatusMessage)
                ? "Соберите маршрут."
                : _executor.StatusMessage;
            _statusText.color = _executor.AttemptsRemaining <= 0 ? new Color(0.9f, 0.35f, 0.35f, 1f) : Color.white;

            int commandCount = _queue.Commands.Count;
            for (int index = 0; index < _queueSlotTexts.Count; index++)
            {
                Text slotText = _queueSlotTexts[index];
                Image slotImage = slotText.transform.parent.GetComponent<Image>();

                if (index < commandCount)
                {
                    RouteCommand command = _queue.Commands[index];
                    slotText.text = $"{index + 1}. {RouteMiniGameIcons.Command(command.Type)}  {RouteDirectionUtility.CommandReadable(command.Type)}";
                    slotImage.color = _executor.LastFailedCommandIndex >= 0 && index >= _executor.LastFailedCommandIndex
                        ? _slotFailedColor
                        : _slotFilledColor;
                }
                else
                {
                    slotText.text = $"{index + 1}. ·";
                    slotImage.color = _slotIdleColor;
                }
            }

            bool canEdit = !_executor.IsRunning;

            SetButtonInteractable(RouteControlAction.MoveUp, canEdit);
            SetButtonInteractable(RouteControlAction.MoveRight, canEdit);
            SetButtonInteractable(RouteControlAction.MoveDown, canEdit);
            SetButtonInteractable(RouteControlAction.MoveLeft, canEdit);
            SetButtonInteractable(RouteControlAction.Wait, canEdit);
            SetButtonInteractable(RouteControlAction.Undo, canEdit);
            SetButtonInteractable(RouteControlAction.Run, canEdit && _executor.AttemptsRemaining > 0);
            SetButtonInteractable(RouteControlAction.Reset, canEdit);
        }

        private IEnumerator FlashRoutine(Image image)
        {
            Color original = image.color;
            image.color = _buttonActiveColor;
            yield return new WaitForSeconds(0.12f);
            image.color = original;
        }

        private void SetButtonInteractable(RouteControlAction action, bool value)
        {
            if (!_buttonImages.TryGetValue(action, out Image image) || image == null)
            {
                return;
            }

            Button button = image.GetComponent<Button>();
            if (button == null)
            {
                return;
            }

            button.interactable = value;
            image.color = value ? _buttonIdleColor : new Color(_buttonIdleColor.r, _buttonIdleColor.g, _buttonIdleColor.b, 0.45f);
        }

        private static RectTransform CreatePanel(string objectName, Transform parent, Color background)
        {
            GameObject panel = new(objectName, typeof(RectTransform), typeof(Image));
            panel.transform.SetParent(parent, false);
            Image image = panel.GetComponent<Image>();
            image.color = background;
            RectTransform rect = panel.GetComponent<RectTransform>();
            rect.localScale = Vector3.one;
            return rect;
        }

        private static Text CreateText(Transform parent, string objectName, string textValue, int fontSize, FontStyle fontStyle, TextAnchor alignment)
        {
            GameObject textObject = new(objectName, typeof(RectTransform), typeof(Text));
            textObject.transform.SetParent(parent, false);

            Text text = textObject.GetComponent<Text>();
            text.font = LoadRuntimeFont();
            text.text = textValue;
            text.fontSize = fontSize;
            text.fontStyle = fontStyle;
            text.alignment = alignment;
            text.color = Color.white;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;

            RectTransform rect = text.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.offsetMin = new Vector2(10f, 6f);
            rect.offsetMax = new Vector2(-10f, -6f);
            rect.localScale = Vector3.one;

            return text;
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

        private static void EnsureEventSystem()
        {
            if (FindObjectOfType<EventSystem>() != null)
            {
                return;
            }

            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        }

        private string GetActionKeyLabel(RouteControlAction action)
        {
            return _inputHandler != null
                ? _inputHandler.GetActionKeyLabel(action)
                : RouteMiniGameIcons.ActionKey(action);
        }
    }
}