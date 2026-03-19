using UnityEngine;
using UnityEngine.UI;

public class MiniGameInputHandler : MonoBehaviour
{
    public CommandQueue queue;
    public ExecutionManager executor;

    [Header("Input Mapping")]
    [SerializeField] private bool swapHorizontalControls;
    [SerializeField] private bool swapVerticalControls;

    private RouteMiniGameHUD _hud;

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
    }

    private void Start()
    {
        EnsureHud();
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
}
