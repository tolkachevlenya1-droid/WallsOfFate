using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class GameProcess : MonoBehaviour {
    [Header("References")]
    [SerializeField] private MineSpawner _mineSpawner;
    [SerializeField] private PlayerMove _playerMove;
    [SerializeField] private AIController _enemyController;

    private IReadOnlyList<Mine> _healMines;
    private IReadOnlyList<Mine> _damageMines;
    private IReadOnlyList<Mine> _buffMines;
    private IReadOnlyList<Mine> _debuffMines;

    public MiniGamePlayer PlayerChar { get; private set; }
    public MiniGamePlayer EnemyChar { get; private set; }

    private bool _isInitialized = false;

    public event Action<bool> OnEndGame;


    public void Initialize() {
        if (_isInitialized) return;

        if (_mineSpawner == null) _mineSpawner = FindFirstObjectByType<MineSpawner>();
        if (_playerMove == null) _playerMove = FindFirstObjectByType<PlayerMove>();
        if (_enemyController == null) _enemyController = FindFirstObjectByType<AIController>();

        InitializeLogic();
        _isInitialized = true;
    }

    private void InitializeLogic() {
        if (_playerMove == null || _enemyController == null) {
            Debug.LogError("Не удалось найти PlayerMove или AIController!");
            return;
        }

        PlayerChar = _playerMove.GetComponent<MiniGamePlayer>();
        EnemyChar = _enemyController.GetComponent<MiniGamePlayer>();

        if (PlayerChar != null && _playerMove != null) {
            PlayerChar.OnSpeedChanged -= _playerMove.ChangeSpeed;
            PlayerChar.OnSpeedChanged += _playerMove.ChangeSpeed;
        }

        if (EnemyChar != null && _enemyController != null) {
            EnemyChar.OnSpeedChanged -= _enemyController.ChangeSpeed;
            EnemyChar.OnSpeedChanged += _enemyController.ChangeSpeed;
        }

        // Получаем списки мин
        _healMines = _mineSpawner.HealMines;
        _damageMines = _mineSpawner.DamageMines;
        _buffMines = _mineSpawner.BuffMines;
        _debuffMines = _mineSpawner.DebuffMines;

        // Подписываемся на события мин
        SubscribeToMineEvents(_healMines);
        SubscribeToMineEvents(_damageMines);
        SubscribeToMineEvents(_buffMines);
        SubscribeToMineEvents(_debuffMines);
    }

    private void FixedUpdate() {
        if (!_isInitialized || PlayerChar == null || EnemyChar == null) return;

        if ((PlayerChar.Health <= 0 && !PlayerChar.isDead) ||
            (EnemyChar.Health <= 0 && !EnemyChar.isDead)) {
            bool playerWin;
            if (PlayerChar.Health > 0) {
                playerWin = true;
            }
            else {
                playerWin = false;
            }

            PlayerChar.isDead = true;
            EnemyChar.isDead = true;

            OnEndGame?.Invoke(playerWin);
        }
    }


    private void SubscribeToMineEvents(IEnumerable<Mine> mines) {
        if (mines == null) return;

        foreach (Mine mine in mines) {
            GameObject minePrefab = mine.MineGameObject;
            if (minePrefab == null) continue;

            TriggerHandler mineTriggerHandler = minePrefab.GetComponent<TriggerHandler>();
            if (mineTriggerHandler != null) {
                // Удаляем старые подписки и добавляем новые
                mineTriggerHandler.OnObjectEnteredTrigger -= HandleMineTrigger;
                mineTriggerHandler.OnObjectEnteredTrigger += HandleMineTrigger;
            }
        }
    }

    private void HandleMineTrigger(GameObject triggeredObject, GameObject objectWhoTriger) {
        Mine mine = FindMineByGameObject(triggeredObject);
        if (mine != null) {
            HandleMineTriggered(mine, objectWhoTriger);
        }
    }

    private Mine FindMineByGameObject(GameObject triggeredObject) {
        if (_healMines != null) {
            var mine = FindMineInList(triggeredObject, _healMines);
            if (mine != null) return mine;
        }

        if (_damageMines != null) {
            var mine = FindMineInList(triggeredObject, _damageMines);
            if (mine != null) return mine;
        }

        if (_buffMines != null) {
            var mine = FindMineInList(triggeredObject, _buffMines);
            if (mine != null) return mine;
        }

        if (_debuffMines != null) {
            var mine = FindMineInList(triggeredObject, _debuffMines);
            if (mine != null) return mine;
        }

        return null;
    }

    private Mine FindMineInList(GameObject triggeredObject, IEnumerable<Mine> mines) {
        foreach (Mine mine in mines) {
            if (mine.MineGameObject == triggeredObject) {
                return mine;
            }
        }
        return null;
    }

    private void HandleMineTriggered(Mine givedMine, GameObject givedPlayer) {
        if (PlayerChar == null || EnemyChar == null) return;

        MiniGamePlayer givedPlayerChar = givedPlayer.GetComponent<MiniGamePlayer>();
        if (givedPlayerChar == null) return;

        if (givedMine is HealMine healMine) {
            healMine.Heal(givedPlayerChar);
        }
        else if (givedMine is DamageMine damageMine) {
            if (givedPlayerChar.Name == "Player")
                damageMine.Damage(EnemyChar, PlayerChar);
            else
                damageMine.Damage(PlayerChar, EnemyChar);
        }
        else if (givedMine is BuffSpeedMine buffSpeedMine) {
            MineExplosion(buffSpeedMine, _playerMove.gameObject, _enemyController.gameObject);
        }

        givedMine.SetActive(false);
    }

    private async void MineExplosion(BuffSpeedMine mine, params GameObject[] objects) {
        if (mine == null) return;

        Vector3 initialMinePosition = mine.MineGameObject.transform.position;
        await Task.Delay(mine.GetTimeBeforeExplosion());

        List<MiniGamePlayer> affectedPlayers = mine.FindDistanceToMine(initialMinePosition, objects);
        await mine.BuffSpeedList(affectedPlayers);
    }

    private void OnDestroy() {
        if (PlayerChar != null && _playerMove != null) {
            PlayerChar.OnSpeedChanged -= _playerMove.ChangeSpeed;
        }
        if (EnemyChar != null && _enemyController != null) {
            EnemyChar.OnSpeedChanged -= _enemyController.ChangeSpeed;
        }
    }
}