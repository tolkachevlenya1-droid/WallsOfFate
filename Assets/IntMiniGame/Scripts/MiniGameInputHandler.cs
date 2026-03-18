using UnityEngine;

public class MiniGameInputHandler : MonoBehaviour
{
    public CommandQueue queue;
    public ExecutionManager executor;

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
        if (_hud == null)
        {
            _hud = FindObjectOfType<RouteMiniGameHUD>();
        }

        if (_hud == null)
        {
            GameObject hudObject = new("RouteMiniGameHUD");
            _hud = hudObject.AddComponent<RouteMiniGameHUD>();
        }

        _hud.Initialize(this, queue, executor);
    }

    private void Update()
    {
        if (queue == null || executor == null)
        {
            return;
        }

        if (WasPressed(KeyCode.W, KeyCode.UpArrow))
        {
            HandleAction(RouteControlAction.MoveUp);
        }
        else if (WasPressed(KeyCode.D, KeyCode.RightArrow))
        {
            HandleAction(RouteControlAction.MoveRight);
        }
        else if (WasPressed(KeyCode.S, KeyCode.DownArrow))
        {
            HandleAction(RouteControlAction.MoveDown);
        }
        else if (WasPressed(KeyCode.A, KeyCode.LeftArrow))
        {
            HandleAction(RouteControlAction.MoveLeft);
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
