using UnityEngine;
using System.Collections.Generic;
using Game;
using Zenject;
using Game.Data;
using System.Linq;

public class AudienceSessionSpawner : MonoBehaviour
{
    /* ───── точки на сцене ───── */
    [Header("Points")]
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private Transform _dialogSpot;
    [SerializeField] private Transform _exitSpot;
    [SerializeField] private DialogueManager _dialogueManager;

    private readonly Queue<NPCDefinition> _sessionQueue = new();
    private NPCController _current;

    /* ───────────────────────────────────────────────────────────── */

    private LoadingManager loadingManager;
    private GameflowManager gameflowManager;
    private QuestManager questManager;

    [Inject]
    private void Construct(LoadingManager loadingManager, GameflowManager gameflowManager, QuestManager questManager)
    {
        this.loadingManager = loadingManager;
        this.gameflowManager = gameflowManager;
        this.questManager = questManager;
    }

    private void Start()
    {
        PrepareQueueAndStart();
    }

    /* ─────────── подготовка очереди ─────────── */
    private void PrepareQueueAndStart()
    {
        // 1) три случайных из мешка
        // TODO: Пока только один для теста
        for (int i = 0; i < 1; i++)
            if (AudiencePool.Instance.TryTakeRandom(out var def))
                _sessionQueue.Enqueue(def);

        var mainQuest = gameflowManager.GetCurrentDayQuests().FirstOrDefault(quest => quest.Main);

        if (mainQuest != null)
        {
            var mainQuestGiver = Resources.Load<NPCDefinition>("Characters/StartDayPrefabs/ScriptableObjects/Quest_" + mainQuest.Id + "_npc");
            _sessionQueue.Enqueue(mainQuestGiver);
        }

        if (_sessionQueue.Count == 0)
        {
            EndSession();        // никого нет → сразу выходим
            return;
        }

        _dialogueManager.OnFinished += OnDialogueFinished;
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
        go.name = def.npcName;

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
        this.loadingManager.LoadSceneAsync("MainRoom");
    }
}
