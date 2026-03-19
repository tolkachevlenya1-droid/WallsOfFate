using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Game.MiniGame.Agility
{
    public class GameProcess : MonoBehaviour
    {
        [Header("Patterns")]
        public AttackPattern EnemyAttackLogic;

        [Header("Game Configs")]
        public float GameTime;
        public int PayerHP;
        public float TimeBetweenAttacks = 1f;
        public Transform SplineSpawnPoint;
        private MiniGameData gameData;

        [Header("Installer")]
        public AgentsFactory agentsFactory;

        [Header("Agents")]
        public GameObject PlayerInstance;
        public GameObject EnemyInstance;

        [Header("UI Managers")]
        [SerializeField] private SliderTimerManager sliderTimerManager;
        [SerializeField] private PlayerHPManager playerHPManager;

        public void Configure(AgentsFactory npcFActory, MiniGameData gameData)
        {
            this.agentsFactory = npcFActory;
            this.gameData = gameData;
        }

        public void Initialize()
        {
            if (agentsFactory == null)
            {
                Debug.LogWarning("GameProcess: AgentsFactory не назначен. Legacy-процесс пропущен.");
                enabled = false;
                return;
            }

            PlayerInstance = agentsFactory.Player;
            EnemyInstance = agentsFactory.Enemy;

            if (PlayerInstance == null || PlayerInstance == null)
            {
                Debug.LogError("GameProcess: Player или Enemy не созданы!");
                return;
            }

            var trigger = PlayerInstance.GetComponent<TriggerHandler>();
            if (trigger != null)
            {
                trigger.OnObjectEnteredTrigger += PlayerHit;
            }

            EnemyAttackLogic = EnemyInstance.GetComponent<AttackPattern>();

            sliderTimerManager.InitializeTimer(GameTime);
            playerHPManager.InitializeHP(PayerHP);

            StartCoroutine(GameLoopCoroutine());
        }

        public void PlayerHit(GameObject player, GameObject collidedObj)
        {
            PayerHP -= 1;
            playerHPManager.UpdateHP(PayerHP);
        }

        private IEnumerator GameLoopCoroutine()
        {
            float startTime = Time.time;
            float endTime = startTime + GameTime;


            while (PayerHP > 0 && Time.time < endTime)
            {
                sliderTimerManager.UpdateTimer(Time.time);

                PatternController nextPattern = EnemyAttackLogic.GetNextPattern();

                if (nextPattern != null)
                {
                    PatternController activePattern = SpawnPattern(nextPattern);

                    yield return new WaitForSeconds(activePattern.LifeTime);

                    activePattern.Cleanup();
                    Destroy(activePattern.gameObject);

                    yield return new WaitForSeconds(TimeBetweenAttacks);
                }
                else
                {
                    yield return null;
                }
            }

            HandleEnd();
        }

        private void HandleEnd()
        {
            bool playerWin = false;
            if (PayerHP > 0) playerWin = true;
            MinigameManager.Instance.EndMinigame(playerWin);
        }

        private PatternController SpawnPattern(PatternController patternPrefab)
        {
            return Instantiate(patternPrefab, SplineSpawnPoint.position, Quaternion.identity, this.transform);

        }
    }
}
