using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class MiniGameInstaller : MonoInstaller
{
    [Header("Player Settings")]
    public Transform StartPoint;
    public GameObject PlayerPrefab;
    public Transform Parent;
    public Transform CameraTransform;

    [Header("HP Bar Settings")]
    public Transform CanvasTransform;
    public Slider PlayerHPBarPrefab;
    public Slider EnemyHPBarPrefab;

    [Header("Enemy Settings")]
    public Transform SpawnPoint;
    public GameObject EnemyPrefab;

    [Header("Game Settings")]
    public MineSpawner MineSpawnerObject;

    private PlayerMove _playerInstance;
    private AIController _enemyInstance;

    private void OnEnable()
    {
        ResetGame();
    }

    public override void InstallBindings()
    {
        if (PlayerHPBarPrefab == null || EnemyHPBarPrefab == null || CanvasTransform == null)
        {
            //Debug.LogError("HP Bar settings are not set!", this);
            return;
        }

        BindCameraTransform();
        BindMineSpawner();
        BindPlayer();
        BindEnemy();
    }

    public void ResetGame()
    {
        if (_enemyInstance != null)
        {
            Destroy(_enemyInstance.gameObject);
        }

        GameObject enemyPrefab = EnemyPrefab;

        // dsПроверяем, нужно ли использовать префаб из DialogueManager
        //if (DialogueManager.HasInstance && DialogueManager.GetInstance().PowerCheckPrefab != null)
        //{
        //    if (enemyPrefab != DialogueManager.GetInstance().PowerCheckPrefab)
        //    {
        //        enemyPrefab = DialogueManager.GetInstance().PowerCheckPrefab;
        //    }
        //}

        InstantiateEnemies(enemyPrefab, SpawnPoint);

        var gameProcess = FindObjectOfType<GameProcess>();
        if (gameProcess != null)
        {
            gameProcess.UpdateReferences(_playerInstance, _enemyInstance);
        }
    }

    private void BindMineSpawner()
    {
        Container.Bind<MineSpawner>().FromInstance(MineSpawnerObject).AsSingle().NonLazy();
    }

    private void BindPlayer()
    {
        if (PlayerPrefab == null || StartPoint == null)
        {
            //Debug.LogError("Player prefab or StartPoint is missing!", this);
            return;
        }

        InstantiatePlayer();

        Container.Bind<PlayerMove>().WithId("Player").FromInstance(_playerInstance).AsSingle();
    }

    private void InstantiatePlayer()
    {
        _playerInstance = Instantiate(PlayerPrefab, StartPoint.position, PlayerPrefab.transform.rotation, Parent)
            .GetComponent<PlayerMove>();

        // Создаем и передаем HealthBar
        Slider playerHealthBar = Instantiate(PlayerHPBarPrefab, CanvasTransform);

        HealthBarManager healthBarManager = _playerInstance.GetComponent<HealthBarManager>();
        if (healthBarManager != null)
        {
            healthBarManager.SetHealthBar(playerHealthBar);
        }
    }

    private void BindEnemy()
    {
        Transform spawnPoint = SpawnPoint;
        GameObject enemyPrefab = EnemyPrefab;

        // dsПроверяем, нужно ли использовать префаб из DialogueManager
        //if (DialogueManager.HasInstance && DialogueManager.GetInstance().PowerCheckPrefab != null)
        //{
        //    if (enemyPrefab != DialogueManager.GetInstance().PowerCheckPrefab)
        //    {
        //        enemyPrefab = DialogueManager.GetInstance().PowerCheckPrefab;
        //    }
        //}

        InstantiateEnemies(enemyPrefab, spawnPoint);

        Container.Bind<AIController>().WithId("Enemy").FromInstance(_enemyInstance).AsSingle();
    }

    private void InstantiateEnemies(GameObject enemyPrefab, Transform spawnPoint)
    {
        _enemyInstance = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation, Parent)
            .GetComponent<AIController>();

        // Убираем "(Clone)" из имени
        _enemyInstance.gameObject.name = enemyPrefab.name;

        HealthBarManager healthBarManager = _enemyInstance.GetComponent<HealthBarManager>();
        if (healthBarManager != null)
        {
            Slider enemyHealthBar = Instantiate(EnemyHPBarPrefab, CanvasTransform);
            healthBarManager.SetHealthBar(enemyHealthBar);
        }
    }

    private void BindCameraTransform()
    {
        Container
            .Bind<Transform>()
            .FromInstance(CameraTransform)
            .WhenInjectedInto<PlayerMove>();
    }
}