using System;
using System.Collections;
using Game;
using Game.MiniGame;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class ExecutionManager : MonoBehaviour, IMiniGameInstaller
{
    public PlayerController player;
    public CommandQueue queue;

    [Header("Scene References")]
    [SerializeField] private GridManager grid;
    [SerializeField] private MiniGameInputHandler inputHandler;

    [Header("Game Rules")]
    [SerializeField] private int maxAttempts = 3;
    [SerializeField] private float betweenCommandsDelay = 0.12f;
    [SerializeField] private float postRunPause = 0.35f;
    [SerializeField] private bool requireExitToWin;
    [SerializeField] private RouteDirection startingDirection = RouteDirection.Up;

    public event Action StateChanged;

    public bool IsRunning { get; private set; }
    public bool IsResolved { get; private set; }
    public int AttemptsRemaining { get; private set; }
    public int MaxAttempts => maxAttempts;
    public string StatusMessage { get; private set; }
    public GridManager Grid => grid;

    private MiniGameData _gameData;
    private bool _initialized;

    public void InitializeWithData(MiniGameData gameData)
    {
        _gameData = gameData;
        EnsureReferences();
        ApplyGameData();
        InitializeSession();
    }

    public void OnMiniGameEnded(bool playerWin)
    {
        if (MinigameManager.Instance != null)
        {
            MinigameManager.Instance.EndMinigame(playerWin);
        }
    }

    public bool TryStartRun()
    {
        if (IsRunning)
        {
            return false;
        }

        if (AttemptsRemaining <= 0)
        {
            SetStatus("Попытки закончились. Нажмите Reset, чтобы начать заново.", true);
            return false;
        }

        if (!queue.ValidateForRun(out string reason))
        {
            SetStatus(reason, true);
            return false;
        }

        StartCoroutine(RunRoutine());
        return true;
    }

    public void ResetSession(bool restoreAttempts, bool clearQueue)
    {
        if (IsRunning)
        {
            return;
        }

        InitializeSession();

        if (restoreAttempts)
        {
            AttemptsRemaining = maxAttempts;
            IsResolved = false;
        }

        if (clearQueue)
        {
            queue.Clear();
        }

        grid.ResetBoardState();
        player.ResetToStart();
        SetStatus("Маршрут сброшен. Составьте новый прогон.", false);
    }

    public void SetStatus(string message, bool isWarning)
    {
        StatusMessage = message;
        StateChanged?.Invoke();

        if (isWarning)
        {
            Debug.LogWarning(message);
        }
        else
        {
            Debug.Log(message);
        }
    }

    private void Awake()
    {
        EnsureReferences();
    }

    private void Start()
    {
        InitializeSession();
    }

    private IEnumerator RunRoutine()
    {
        IsRunning = true;
        SetStatus("Прогон запущен. Магнат выполняет маршрут...", false);

        grid.ResetBoardState();
        player.ResetToStart();

        if (!ValidateStartCell(out string startFailure))
        {
            yield return StartCoroutine(HandleFailure(startFailure));
            yield break;
        }

        for (int commandIndex = 0; commandIndex < queue.Commands.Count; commandIndex++)
        {
            RouteCommand command = queue.Commands[commandIndex];

            if (RouteDirectionUtility.TryGetStepDirection(command.Type, out RouteDirection stepDirection))
            {
                for (int step = 0; step < command.Value; step++)
                {
                    if (player.FacingDirection != stepDirection)
                    {
                        yield return player.AnimateTurn(stepDirection);
                    }

                    Vector2Int nextPosition = player.PeekPosition(stepDirection);

                    if (!grid.IsInside(nextPosition))
                    {
                        yield return StartCoroutine(HandleFailure("Магнат вышел за границы поля."));
                        yield break;
                    }

                    if (grid.IsBlocked(nextPosition))
                    {
                        yield return StartCoroutine(HandleFailure("Столкновение со стеной или блокирующим объектом."));
                        yield break;
                    }

                    yield return player.AnimateMoveTo(nextPosition);

                    int collected = grid.CollectArguments(nextPosition);
                    if (collected > 0)
                    {
                        SetStatus(collected > 1
                            ? $"Собрано доводов: +{collected}. Осталось {grid.RemainingArguments}."
                            : $"Довод собран. Осталось {grid.RemainingArguments}.", false);
                    }

                    if (grid.IsForbidden(nextPosition))
                    {
                        yield return StartCoroutine(HandleFailure("Вход в запрещённую клетку."));
                        yield break;
                    }

                    yield return new WaitForSeconds(betweenCommandsDelay);
                }

                continue;
            }

            if (command.Type == RouteCommandType.Wait)
            {
                SetStatus("Пауза.", false);
                yield return new WaitForSeconds(Mathf.Max(betweenCommandsDelay, 0.18f) * command.Value);
            }
        }

        if (grid.RemainingArguments > 0)
        {
            yield return StartCoroutine(HandleFailure($"Не все доводы собраны. Осталось {grid.RemainingArguments}."));
            yield break;
        }

        if (requireExitToWin && grid.HasExitCell && !grid.IsExit(player.gridPosition))
        {
            yield return StartCoroutine(HandleFailure("Все доводы собраны, но Магнат не завершил маршрут в выходной клетке."));
            yield break;
        }

        yield return new WaitForSeconds(postRunPause);
        HandleVictory();
    }

    private IEnumerator HandleFailure(string message)
    {
        AttemptsRemaining = Mathf.Max(0, AttemptsRemaining - 1);
        IsRunning = false;

        if (AttemptsRemaining > 0)
        {
            SetStatus($"{message} Осталось попыток: {AttemptsRemaining}/{maxAttempts}.", true);
            yield return new WaitForSeconds(postRunPause);
            grid.ResetBoardState();
            player.ResetToStart();
            StateChanged?.Invoke();
            yield break;
        }

        IsResolved = true;
        SetStatus($"{message} Попытки закончились.", true);
        yield return new WaitForSeconds(postRunPause);

        if (MinigameManager.Instance != null)
        {
            OnMiniGameEnded(false);
        }
    }

    private void HandleVictory()
    {
        IsRunning = false;
        IsResolved = true;
        SetStatus("Все доводы собраны. Прогон успешен.", false);

        if (MinigameManager.Instance != null)
        {
            OnMiniGameEnded(true);
        }
    }

    private void InitializeSession()
    {
        EnsureReferences();

        if (grid == null || player == null || queue == null)
        {
            SetStatus("Сцена мини-игры настроена не полностью: не хватает ссылок на grid, player или queue.", true);
            return;
        }

        grid.RefreshLayout();
        player.Initialize(grid);
        player.SetStartingDirection(startingDirection, true);

        if (!_initialized)
        {
            AttemptsRemaining = maxAttempts;
            _initialized = true;
        }
        else if (AttemptsRemaining <= 0 && !IsResolved)
        {
            AttemptsRemaining = maxAttempts;
        }

        grid.ResetBoardState();
        player.ResetToStart();
        IsRunning = false;

        if (!IsResolved || AttemptsRemaining > 0)
        {
            SetStatus("Соберите маршрут и нажмите Enter, чтобы запустить прогон.", false);
        }
    }

    private void EnsureReferences()
    {
        if (grid == null)
        {
            grid = FindObjectOfType<GridManager>();
        }

        if (player == null)
        {
            player = FindObjectOfType<PlayerController>();
        }

        if (queue == null)
        {
            queue = FindObjectOfType<CommandQueue>();
        }

        if (inputHandler == null)
        {
            inputHandler = FindObjectOfType<MiniGameInputHandler>();
        }
    }

    private bool ValidateStartCell(out string reason)
    {
        if (!grid.IsInside(player.gridPosition))
        {
            reason = "Стартовая позиция находится вне поля.";
            return false;
        }

        if (grid.IsBlocked(player.gridPosition))
        {
            reason = "Стартовая позиция занята стеной.";
            return false;
        }

        if (grid.IsForbidden(player.gridPosition))
        {
            reason = "Стартовая позиция находится в запрещённой клетке.";
            return false;
        }

        reason = string.Empty;
        return true;
    }

    private void ApplyGameData()
    {
        if (_gameData == null || _gameData.customParameters == null)
        {
            return;
        }

        if (TryGetInt("attempts", out int attempts))
        {
            maxAttempts = Mathf.Max(1, attempts);
            AttemptsRemaining = maxAttempts;
        }

        int? queueLimit = TryGetInt("maxCommands", out int maxCommandsValue) ? maxCommandsValue : null;
        int? turnLimit = TryGetInt("maxTurns", out int maxTurnsValue) ? maxTurnsValue : null;
        int? waitLimit = TryGetInt("maxWaits", out int maxWaitsValue) ? maxWaitsValue : null;
        int? moveLimit = TryGetInt("maxMoves", out int maxMovesValue) ? maxMovesValue : null;
        int? requiredCount = TryGetInt("requiredCommandCount", out int requiredCommandCountValue) ? requiredCommandCountValue : null;

        RouteCommandType? requiredCommand = null;
        if (TryGetString("requiredCommand", out string requiredCommandText) &&
            Enum.TryParse(requiredCommandText, true, out RouteCommandType parsedCommand))
        {
            requiredCommand = parsedCommand;
        }

        queue.ApplyExternalLimits(queueLimit, turnLimit, waitLimit, moveLimit, requiredCommand, requiredCount);

        if (TryGetFloat("stepDelay", out float commandDelay))
        {
            betweenCommandsDelay = Mathf.Max(0f, commandDelay);
        }

        if (TryGetBool("requireExit", out bool requireExit))
        {
            requireExitToWin = requireExit;
        }

        if (TryGetString("startingDirection", out string directionText) &&
            Enum.TryParse(directionText, true, out RouteDirection parsedDirection))
        {
            startingDirection = parsedDirection;
        }
    }

    private bool TryGetInt(string key, out int value)
    {
        value = 0;
        if (!TryGetObject(key, out object rawValue))
        {
            return false;
        }

        if (rawValue is int intValue)
        {
            value = intValue;
            return true;
        }

        if (rawValue is long longValue)
        {
            value = (int)longValue;
            return true;
        }

        if (rawValue is float floatValue)
        {
            value = Mathf.RoundToInt(floatValue);
            return true;
        }

        if (rawValue is double doubleValue)
        {
            value = (int)Math.Round(doubleValue);
            return true;
        }

        if (rawValue is string stringValue && int.TryParse(stringValue, out int parsedValue))
        {
            value = parsedValue;
            return true;
        }

        if (rawValue is JValue jValue)
        {
            value = jValue.Value<int>();
            return true;
        }

        return false;
    }

    private bool TryGetFloat(string key, out float value)
    {
        value = 0f;
        if (!TryGetObject(key, out object rawValue))
        {
            return false;
        }

        if (rawValue is float floatValue)
        {
            value = floatValue;
            return true;
        }

        if (rawValue is double doubleValue)
        {
            value = (float)doubleValue;
            return true;
        }

        if (rawValue is int intValue)
        {
            value = intValue;
            return true;
        }

        if (rawValue is long longValue)
        {
            value = longValue;
            return true;
        }

        if (rawValue is string stringValue && float.TryParse(stringValue, out float parsedValue))
        {
            value = parsedValue;
            return true;
        }

        if (rawValue is JValue jValue)
        {
            value = jValue.Value<float>();
            return true;
        }

        return false;
    }

    private bool TryGetBool(string key, out bool value)
    {
        value = false;
        if (!TryGetObject(key, out object rawValue))
        {
            return false;
        }

        if (rawValue is bool boolValue)
        {
            value = boolValue;
            return true;
        }

        if (rawValue is string stringValue && bool.TryParse(stringValue, out bool parsedValue))
        {
            value = parsedValue;
            return true;
        }

        if (rawValue is JValue jValue)
        {
            value = jValue.Value<bool>();
            return true;
        }

        return false;
    }

    private bool TryGetString(string key, out string value)
    {
        value = string.Empty;
        if (!TryGetObject(key, out object rawValue) || rawValue == null)
        {
            return false;
        }

        if (rawValue is string stringValue)
        {
            value = stringValue;
            return true;
        }

        if (rawValue is JValue jValue)
        {
            value = jValue.Value<string>();
            return true;
        }

        value = rawValue.ToString();
        return !string.IsNullOrEmpty(value);
    }

    private bool TryGetObject(string key, out object rawValue)
    {
        rawValue = null;
        return _gameData != null &&
               _gameData.customParameters != null &&
               _gameData.customParameters.TryGetValue(key, out rawValue);
    }
}
