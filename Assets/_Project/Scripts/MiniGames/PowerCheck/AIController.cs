using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace Game.MiniGame.PowerCheck
{
    /// <summary>AI that выбирает ближайшую «выгодную» мину и перемещается к ней.</summary>
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(MiniGamePlayer))]
    public class AIController : MonoBehaviour
    {
        #region Inspector
        [Header("References")]
        [SerializeField] private MineSpawner _mineSpawner;
        [SerializeField] private MiniGamePlayer _thisStats;
        [SerializeField] private MiniGamePlayer _playerStats;
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private Transform _playerTf;

        [Header("Global Modifiers")]
        [SerializeField] private float _damageGlobalModifier = 1f;
        [SerializeField] private float _healGlobalModifier = 1f;
        [SerializeField] private float _buffGlobalModifier = 1f;
        [SerializeField] private float _slowness = 1f;

        [Header("Risk Calculation")]
        [SerializeField] private float _riskDistanceThreshold = 5f;
        [SerializeField] private float _playerRiskFactor = 15f;
        [SerializeField] private float _mineRiskFactor = 5f;

        [Header("Movement")]
        [SerializeField, Range(0.2f, 5f)] private float _stuckTimeThreshold = 1f;
        [SerializeField] private float _stuckVelocitySqrThreshold = 0.01f;
        [SerializeField] private float _speedSmoothTime = 0.2f;
        #endregion

        private readonly Queue<Vector3> _targets = new();
        private readonly List<Mine> _buffer = new(16);

        private float _baseSpeed;
        private float _targetSpeed;
        private Vector3 _currentTarget;
        private float _stuckTimer;

        // counts to detect spawn/despawn
        private int _countDam, _countHeal, _countBuff;
        private bool _initialized = false;

        // ───────────────────────── LIFECYCLE ─────────────────────────
        private void Awake()
        {
            RefreshFields();
        }

        private void RefreshFields()
        {
            if (!_mineSpawner) _mineSpawner = FindFirstObjectByType<MineSpawner>();
            if (!_thisStats) _thisStats = GetComponent<MiniGamePlayer>();
            if (!_agent) _agent = GetComponent<NavMeshAgent>();

            if (!_playerStats || !_playerTf)
            {
                PlayerMove player = FindFirstObjectByType<PlayerMove>();
                if (player != null)
                {
                    _playerTf = player.transform;
                    _playerStats = player.GetComponent<MiniGamePlayer>();
                }
            }
        }

        private void Start()
        {
            if (!_agent.isOnNavMesh &&
                NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 2f, NavMesh.AllAreas))
                _agent.Warp(hit.position);

            _agent.updateRotation = true;
            _agent.autoBraking = true;

            _baseSpeed = _thisStats.Speed;
            _targetSpeed = _baseSpeed * _slowness;
            _agent.speed = 0f;

            RebuildTargets();
        }

        private void Update()
        {
            if (!_initialized)
            {
                RefreshFields();
                _initialized = true;
            }
            _agent.speed = Mathf.Lerp(_agent.speed, _targetSpeed, Time.deltaTime / _speedSmoothTime);

            if (!_agent.pathPending && (_targets.Count == 0 || PickUpsChanged()))
                RebuildTargets();

            if (!_agent.pathPending && _targets.Count > 0 &&
                (!_agent.hasPath || _agent.remainingDistance <= _agent.stoppingDistance || !MineExists(_currentTarget)))
                SetNextDestination();

            if (_agent.hasPath && _agent.velocity.sqrMagnitude < _stuckVelocitySqrThreshold)
            {
                _stuckTimer += Time.deltaTime;
                if (_stuckTimer > _stuckTimeThreshold)
                {
                    _stuckTimer = 0f;
                    RebuildTargets();
                }
            }
            else _stuckTimer = 0f;
        }

        // ───────────────────────── TARGETING ─────────────────────────
        private void RebuildTargets()
        {
            _targets.Clear();
            StartCoroutine(BuildTargets());
        }

        private IEnumerator BuildTargets()
        {
            yield return null;                         // frame delay

            _buffer.Clear();
            AddBest(_mineSpawner.DamageMines);
            AddBest(_mineSpawner.HealMines);
            AddBest(_mineSpawner.BuffMines);

            _buffer.Sort((a, b) => EvaluateMine(a).CompareTo(EvaluateMine(b)));
            foreach (var m in _buffer)
                _targets.Enqueue(m.MineGameObject.transform.position);

            SetNextDestination();
        }

        private void AddBest(IReadOnlyList<Mine> mines)
        {
            Mine best = null;
            float bestScore = float.MaxValue;

            foreach (var m in mines)
            {
                if (!m.Active) continue;
                float s = EvaluateMine(m);
                if (s < bestScore) { bestScore = s; best = m; }
            }
            if (best != null) _buffer.Add(best);
        }

        private void SetNextDestination()
        {
            while (_targets.Count > 0)
            {
                _currentTarget = _targets.Dequeue();
                if (!MineExists(_currentTarget)) continue;

                if (NavMesh.SamplePosition(_currentTarget, out NavMeshHit hit, 1f, NavMesh.AllAreas))
                {
                    _agent.SetDestination(hit.position);
                    return;
                }
            }
            RebuildTargets();
        }

        // ───────────────────────── EVALUATION ─────────────────────────
        private float EvaluateMine(Mine mine)
        {
            float playerHP = _playerStats.Health / _playerStats.MaxHealth;
            float thisHP = _thisStats.Health / _thisStats.MaxHealth;

            float value = mine switch
            {
                DamageMine => EvalDamage(playerHP, thisHP),
                HealMine => EvalHeal(thisHP),
                BuffSpeedMine b => EvalBuff(b),
                _ => 0f
            };

            float dist = Vector3.Distance(transform.position, mine.MineGameObject.transform.position) + 0.1f;
            float risk = CalcRisk(mine);
            return (value - risk) * dist;              // lower → higher priority
        }

        private float EvalBuff(BuffSpeedMine m)
        {
            float area = _mineSpawner.spawnAreaSize.x * _mineSpawner.spawnAreaSize.y;
            float mineArea = m.MineGameObject.transform.localScale.x *
                              m.MineGameObject.transform.localScale.z;
            float density = (_mineSpawner.DebuffMines.Count * mineArea) / area;

            float mod = density switch { < 0.3f => 1.5f, < 0.7f => 1f, _ => 0.1f };
            return 5f * m.GetSpeedBuff() * mod * _buffGlobalModifier;
        }

        private float EvalHeal(float hp)
        {
            float mod = hp switch { < 0.3f => 2f, < 0.7f => 1f, _ => 0.5f };
            return 10f * _thisStats.HealingAmount * mod * _healGlobalModifier;
        }

        private float EvalDamage(float playerHP, float thisHP)
        {
            float mod = 1f;
            if (playerHP < 0.3f) mod *= 1.5f;
            if (playerHP > 0.7f) mod *= 0.8f;
            if (thisHP < 0.2f) mod *= 0.5f;
            return 10f * _thisStats.Damage * mod * _damageGlobalModifier;
        }

        private float CalcRisk(Mine mine)
        {
            float p = _playerRiskFactor /
                      (Vector3.Distance(mine.MineGameObject.transform.position, _playerTf.position) + 0.1f);

            foreach (var d in _mineSpawner.DebuffMines)
            {
                float dist = Vector3.Distance(mine.MineGameObject.transform.position, d.MineGameObject.transform.position);
                if (dist < _riskDistanceThreshold) p += _mineRiskFactor / dist;
            }
            return p;
        }

        // ───────────────────────── HELPERS ─────────────────────────
        private bool MineExists(Vector3 pos)
        {
            const float eps = 0.1f;
            return _mineSpawner.HealMines.Any(m => Vector3.Distance(m.MineGameObject.transform.position, pos) <= eps) ||
                   _mineSpawner.DamageMines.Any(m => Vector3.Distance(m.MineGameObject.transform.position, pos) <= eps) ||
                   _mineSpawner.BuffMines.Any(m => Vector3.Distance(m.MineGameObject.transform.position, pos) <= eps);
        }

        private bool PickUpsChanged()
        {
            int dam = _mineSpawner.DamageMines.Count(m => m.Active);
            int heal = _mineSpawner.HealMines.Count(m => m.Active);
            int buff = _mineSpawner.BuffMines.Count(m => m.Active);

            bool changed = dam != _countDam || heal != _countHeal || buff != _countBuff;
            _countDam = dam; _countHeal = heal; _countBuff = buff;
            return changed;
        }

        /// <summary>Внешний вызов: сменить скорость (дебафф / бафф).</summary>
        public void ChangeSpeed(float multiplier, bool isDebuff)
        {
            _targetSpeed = _baseSpeed * multiplier;
            if (isDebuff) _slowness = multiplier;      // сохраняем текущее значение
        }
    }

}

