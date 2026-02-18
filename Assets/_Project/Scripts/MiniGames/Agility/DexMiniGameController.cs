using System;
using System.Collections;
using UnityEngine;

public class DexMiniGameController : MonoBehaviour
{
    [Header("Config")]
    public MiniGameConfig config;
    public DexDifficultyProfile difficultyProfile;
    public PatternDatabase patternDatabase;

    [Header("Refs")]
    public PatternSequencer sequencer;
    public PlayerHealth playerHealth;
    public MovementModifiers playerModifiers;

    [Header("Runtime")]
    public MiniGameState state = MiniGameState.Idle;

    [Tooltip("Dex игрока (подайте сюда из вашей RPG-системы)")]
    public int dex = 5;

    public int seed = 12345;

    private Coroutine _runRoutine;

    public event Action<bool> OnEndGame;

    public void StartMiniGame()
    {
        if (_runRoutine != null) StopCoroutine(_runRoutine);
        _runRoutine = StartCoroutine(RunFlow());
    }

    private IEnumerator RunFlow()
    {
        state = MiniGameState.Countdown;

        playerHealth.ResetTo(config.startingHp, config.iFramesSeconds);

        sequencer.Init(playerHealth, playerModifiers);

        // Countdown
        float cd = config.countdownSeconds;
        while (cd > 0f)
        {
            cd -= Time.deltaTime;
            yield return null;
        }

        state = MiniGameState.SafePhase;
        yield return new WaitForSeconds(config.safePhaseSeconds);

        // Generate RunPlan
        var plan = RunPlanGenerator.Generate(dex, seed, config, difficultyProfile, patternDatabase);

        state = MiniGameState.Running;

        // Run timer parallel to patterns
        float timeLeft = config.runDuration;
        var patternsRoutine = StartCoroutine(sequencer.Play(plan));

        while (timeLeft > 0f && !playerHealth.IsDead)
        {
            timeLeft -= Time.deltaTime;
            yield return null;
        }


        state = playerHealth.IsDead ? MiniGameState.Lose : MiniGameState.Win;
        StopCoroutine(patternsRoutine);
        if (state == MiniGameState.Lose) OnEndGame.Invoke(false);
        else if (state == MiniGameState.Win) OnEndGame.Invoke(true);
    }
}
