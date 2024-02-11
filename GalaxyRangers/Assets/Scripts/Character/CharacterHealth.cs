using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CharacterHealth : MonoBehaviour
{
    [Header("STAT")]
    [Space]
    public CharacterStat Health;
    public float MaxHealth { get { return Health.MaxValue; } }
    public float CurrentHealth { get { return Health.CurrentValue; } }
    [Space]
    [SerializeField] private bool _isDead = false;
    public bool isDead { get { return _isDead; } }
    [SerializeField] private bool _isInvulnerable = false;
    public bool isInvulnerable { get { return _isInvulnerable; } }
    private float _invulnerabilityDurationTimer = 0;

    public delegate void DefaultCallback();
    public event DefaultCallback CharacterDied;
    public event DefaultCallback CharacterHurt;
    public event DefaultCallback CharacterHealed;

    private Coroutine invulnerabilityCoroutine;

    private void Awake()
    {
        Health.CurrentValueReachedZero += OnHealthReachedZero;
        float healthValue = Health.MaxValue;
    }

    public void Hurt(float damage)
    {
        if (_isDead || _isInvulnerable)
            return;
        Health.Damage(damage);

        CharacterHurt?.Invoke();
    }

    public void Invulnerability(float invulnerabilityDuration = .5f)
    {
        if (invulnerabilityDuration <= 0f && _invulnerabilityDurationTimer < invulnerabilityDuration)
            return;
        if (invulnerabilityCoroutine != null)
        {
            StopCoroutine(invulnerabilityCoroutine);
        }
        invulnerabilityCoroutine = StartCoroutine(CoInvulnerability(invulnerabilityDuration));
    }

    private IEnumerator CoInvulnerability(float invulnerabilityDuration)
    {
        _isInvulnerable = true;
        _invulnerabilityDurationTimer += invulnerabilityDuration;
        while (_invulnerabilityDurationTimer > 0)
        {
            _invulnerabilityDurationTimer -= Time.deltaTime;
            yield return null;
        }
        _isInvulnerable = false;
    }

    public void Heal(float heal)
    {
        if (isDead) return;
        Health.Heal(heal);

        CharacterHealed?.Invoke();
    }

    private void OnHealthReachedZero()
    {
        if (_isDead) return;
        Debug.LogWarning(this.gameObject.name + " has died!");
        _isDead = true;

        CharacterDied?.Invoke();
    }
}
