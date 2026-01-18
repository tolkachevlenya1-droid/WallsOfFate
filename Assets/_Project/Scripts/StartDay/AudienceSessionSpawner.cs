using Game.Quest;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Game
{

    public class AudienceSessionSpawner : MonoBehaviour
    {
        /* ───── точки на сцене ───── */
        [Header("Points")]
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private Transform _dialogSpot;
        [SerializeField] private Transform _exitSpot;

        /* ───── босс-НПС для каждого дня ───── */
        [Header("Main Quest Givers  (day 0 / day 1 / day 2)")]
        [SerializeField] private NPCDefinition[] _mainQuestGivers;

        private readonly Queue<NPCDefinition> _sessionQueue = new();
        private NPCController _current;

        /* ───────────────────────────────────────────────────────────── */

        private LoadingManager loadingManager;
        [Inject]
        public void Construct(LoadingManager loadingManager)
        {
            this.loadingManager = loadingManager;
        }

        private void Start()
        {
            // Если открыт loading-экран — дождаться его закрытия
            if (loadingManager.IsLoading)
            {
                loadingManager.LoadingFinished += OnLoadingClosed;
            }
            else
            {
                PrepareQueueAndStart();
            }
        }

        private void OnLoadingClosed()
        {
            loadingManager.LoadingFinished -= OnLoadingClosed;
            PrepareQueueAndStart();
        }

        /* ─────────── подготовка очереди ─────────── */
        private void PrepareQueueAndStart()
        {
            // 1) три случайных из мешка
            for (int i = 0; i < 3; i++)
                if (AudiencePool.Instance.TryTakeRandom(out var def))
                    _sessionQueue.Enqueue(def);

            // 2) босс-проситель сегодняшнего дня
            int day = QuestCollection.CurrentDayNumber;          // 0,1,2 …
            if (day < _mainQuestGivers.Length)
                _sessionQueue.Enqueue(_mainQuestGivers[day]);

            if (_sessionQueue.Count == 0)
            {
                EndSession();        // никого нет → сразу выходим
                return;
            }

            DialogueManager.GetInstance().DialogueFinished += OnDialogueFinished;
            SpawnNext();
        }

        /* ─────────── спавн одного НПС ─────────── */
        private void SpawnNext()
        {
            if (_sessionQueue.Count == 0)
            {
                EndSession();
                return;
            }

            NPCDefinition def = _sessionQueue.Dequeue();
            GameObject go = Instantiate(def.prefab, _spawnPoint.position, _spawnPoint.rotation);

            _current = go.GetComponent<NPCController>();
            _current.Init(_dialogSpot, _exitSpot);

            _current.Left += OnNpcLeft;
        }

        /* ─────────── колбэки ─────────── */
        private void OnDialogueFinished() => _current?.Leave();

        private void OnNpcLeft(NPCController npc)
        {
            npc.Left -= OnNpcLeft;
            Destroy(npc.gameObject, 0.3f);
            SpawnNext();
        }

        private void EndSession()
        {
            ////Debug.Log("<color=yellow>Приём окончен — переходим в MainRoom</color>");
            loadingManager.LoadScene("MainRoom");
        }

        private void OnDestroy()
        {
            if (DialogueManager.HasInstance)
                DialogueManager.GetInstance().DialogueFinished -= OnDialogueFinished;
            
            loadingManager.LoadingFinished -= OnLoadingClosed;
        }
    }

}
