using System;
using System.Collections;
using UnityEngine;
using Zenject;

public class MiniGamePlayer : MonoBehaviour
{
    // ============================
    // Настройки игровых параметров игрока
    // ============================
    [SerializeField] private string playerName;      // Имя игрока
    [SerializeField] private string playerNameForGame;      // Имя игрока
    [SerializeField] private uint maxHealth;         // Максимальное здоровье
    [SerializeField] private uint health;            // Текущее здоровье

    // Новый: минимальное и максимальное значения урона
    [Header("Damage Range")]
    [SerializeField] private uint minDamage = 1;     // Нижняя граница урона
    [SerializeField] private uint damage = 1;        // Верхняя граница урона (раньше — просто damage)

    // Базовая скорость
    [SerializeField] private float speed;
    [SerializeField] private float speedModifier;

    // Новый: минимальное и максимальное значения лечения
    [Header("Healing Range")]
    [SerializeField] private uint minHealingAmount = 1;  // Нижняя граница лечения
    [SerializeField] private uint healingAmount = 1;     // Верхняя граница лечения (раньше — просто healingAmount)

    public bool isDead = false;                     // Флаг смерти

    // ============================
    // Настройки VFX
    // ============================
    [Header("World-Prefab VFX")]
    [SerializeField] private GameObject floatingTextWorldPrefab;
    [SerializeField] private Renderer playerRenderer;
    [SerializeField] private float flashDuration = 0.2f;
    [SerializeField] public string Portrait = "";

    [Header("Pulse Effect")]
    [SerializeField] private float initialBuffPulseDuration = 1.0f;
    [SerializeField] private float initialDebuffPulseDuration = 1.0f;
    [SerializeField] private float minPulseDuration = 0.2f;
    [SerializeField] private float pulseDecayFactor = 0.8f;

    private Player.Stats _playerStats;
    private bool underDebuff;
    private Coroutine pulseCoroutine;

    public string Name
    {
        get => playerName;
        set => playerName = value;
    }

    public uint MaxHealth => maxHealth;
    public uint Health
    {
        get => health;
        set => health = value;
    }

    // Теперь свойство Damage возвращает случайное значение в заданном диапазоне [minDamage; damage]
    public uint Damage
    {
        get
        {
            if (damage <= minDamage)
                return damage;
            int min = (int)minDamage;
            int maxExclusive = (int)damage + 1;
            return (uint)UnityEngine.Random.Range(min, maxExclusive);
        }
    }

    public float SpeedModifier
    {
        get => speedModifier;
        set
        {
            speedModifier = value;
            OnSpeedChanged?.Invoke(speedModifier, underDebuff);
        }
    }

    public float Speed => speed;

    // HealingAmount теперь хранит верхнюю границу,
    // реальное лечение считается внутри метода TakeHeal
    public uint HealingAmount => healingAmount;

    public event Action<float, bool> OnSpeedChanged;

    [Inject]
    public void Construct(Player.Stats playerStats) {
            _playerStats = playerStats;
    }


    private void OnEnable()
    {
        ResetHealth();
        speedModifier += _playerStats.Dex;
        damage += Convert.ToUInt32(_playerStats.Strength);
        minDamage += Convert.ToUInt32(_playerStats.Strength);
    }

    public string GetName() {
        return playerNameForGame;
    }

    public void TakeDamage(uint dmg)
    {
        health = health >= dmg ? health - dmg : 0;
        SpawnFloatingText($"-{dmg}", Color.red);
        StartCoroutine(FlashRoutine(Color.red));
    }

    public void TakeHeal()
    {
        // Вычисляем случайное лечение в диапазоне [minHealingAmount; healingAmount]
        uint healValue;
        if (healingAmount <= minHealingAmount)
        {
            healValue = healingAmount;
        }
        else
        {
            int min = (int)minHealingAmount;
            int maxExc = (int)healingAmount + 1;
            healValue = (uint)UnityEngine.Random.Range(min, maxExc);
        }

        health += healValue;
        if (health > maxHealth)
            health = maxHealth;

        SpawnFloatingText($"+{healValue}", Color.green);
        StartCoroutine(FlashRoutine(Color.green));
    }

    public void TakeSpeedboost(float speedMultiplier, bool isDebuff)
    {
        underDebuff = isDebuff;
        SpeedModifier = speedMultiplier;

        if (pulseCoroutine != null)
        {
            StopCoroutine(pulseCoroutine);
            pulseCoroutine = null;
            if (playerRenderer != null)
                playerRenderer.material.color = Color.white;
        }

        if (!isDebuff)
            SpawnFloatingText("SPEED+", new Color(0f, 1f, 1f));
        else
            SpawnFloatingText("STUN", Color.magenta);

        if (Mathf.Abs(speedMultiplier - 1f) > 0.001f)
        {
            float startDuration = isDebuff ? initialDebuffPulseDuration : initialBuffPulseDuration;
            Color pulseColor = isDebuff ? Color.magenta : new Color(0f, 1f, 1f);
            if (!isDebuff)
                pulseCoroutine = StartCoroutine(DelayedPulseRoutine(pulseColor, startDuration, flashDuration));
            else
                pulseCoroutine = StartCoroutine(PulseRoutine(pulseColor, startDuration));
        }
    }

    private IEnumerator FlashRoutine(Color flashColor)
    {
        if (playerRenderer == null || flashDuration <= 0f)
            yield break;

        var mat = playerRenderer.material;
        var original = mat.color;
        float halfDur = flashDuration * 0.5f;
        float timer = 0f;

        while (timer < halfDur)
        {
            mat.color = Color.Lerp(original, flashColor, timer / halfDur);
            timer += Time.deltaTime;
            yield return null;
        }
        mat.color = flashColor;

        timer = 0f;
        while (timer < halfDur)
        {
            mat.color = Color.Lerp(flashColor, original, timer / halfDur);
            timer += Time.deltaTime;
            yield return null;
        }
        mat.color = original;
    }

    private IEnumerator PulseRoutine(Color pulseColor, float duration)
    {
        if (playerRenderer == null)
            yield break;

        var mat = playerRenderer.material;
        var original = mat.color;
        float currentDur = duration;

        while (currentDur >= minPulseDuration)
        {
            float half = currentDur * 0.5f;
            float timer = 0f;
            while (timer < half)
            {
                mat.color = Color.Lerp(original, pulseColor, timer / half);
                timer += Time.deltaTime;
                yield return null;
            }
            mat.color = pulseColor;

            timer = 0f;
            while (timer < half)
            {
                mat.color = Color.Lerp(pulseColor, original, timer / half);
                timer += Time.deltaTime;
                yield return null;
            }
            mat.color = original;
            currentDur *= pulseDecayFactor;
        }

        mat.color = original;
        pulseCoroutine = null;
    }

    private IEnumerator DelayedPulseRoutine(Color pulseColor, float duration, float delay)
    {
        yield return new WaitForSeconds(delay);
        yield return PulseRoutine(pulseColor, duration);
    }

    private void SpawnFloatingText(string text, Color color)
    {
        var cam = GetPowerCheckCamera();
        if (floatingTextWorldPrefab == null)
            return;

        var go = Instantiate(floatingTextWorldPrefab,
            transform.position + Vector3.up * 1.5f,
            Quaternion.identity);
        var ft = go.GetComponent<FloatingTextMinigame>();
        if (ft != null)
            ft.Setup(text, color, cam);
    }

    private void ResetHealth()
    {
        health = maxHealth;
        isDead = false;
    }

    private Camera GetPowerCheckCamera()
    {
        var go = GameObject.FindGameObjectWithTag("PowerCheckCamera");
        return go != null ? go.GetComponent<Camera>() : null;
    }
}
