using System;
using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int MaxHp = 3;
    public int Hp = 3;

    public bool IsDead => Hp <= 0;
    public bool IsInvulnerable => _invulnerable;

    public event Action<int, int> OnHpChanged; // (hp, max)
    public event Action OnDied;

    private float _iFrames = 0.8f;
    private bool _invulnerable;
    private Coroutine _invulnRoutine;

    public void ResetTo(int hp, float iFramesSeconds)
    {
        if (_invulnRoutine != null)
        {
            StopCoroutine(_invulnRoutine);
            _invulnRoutine = null;
        }

        MaxHp = hp;
        Hp = hp;
        _iFrames = iFramesSeconds;
        _invulnerable = false;
        OnHpChanged?.Invoke(Hp, MaxHp);
    }

    public void TakeDamage(int amount)
    {
        if (IsDead || _invulnerable) return;

        Hp = Mathf.Max(0, Hp - amount);
        OnHpChanged?.Invoke(Hp, MaxHp);

        if (Hp <= 0)
        {
            OnDied?.Invoke();
            return;
        }

        if (_invulnRoutine != null) StopCoroutine(_invulnRoutine);
        _invulnRoutine = StartCoroutine(Invulnerability());
    }

    private IEnumerator Invulnerability()
    {
        _invulnerable = true;
        float t = _iFrames;
        while (t > 0f)
        {
            t -= Time.deltaTime;
            yield return null;
        }
        _invulnerable = false;
        _invulnRoutine = null;
    }
}
