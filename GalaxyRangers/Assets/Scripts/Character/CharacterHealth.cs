using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TypeOfCharacter
{
    Player,
    EnemyCac,
    EnemyCacEpic,
    EnemyLongRange,
    EnemyLongRangeEpic,
    EnemyBoss
}

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

    public delegate void HealthCallback(CharacterHealth characterHealth);
    public delegate void DefaultCallback();
    public event HealthCallback CharacterDied;
    public event HealthCallback CharacterHurt;
    public event HealthCallback CharacterHealed;
    public event HealthCallback CharacterRevived;

// Pas beau mais fonctionne
    [Header("Leezak's Code")]
    [SerializeField] private TypeOfCharacter _typeOfCharacter = TypeOfCharacter.Player;
    [SerializeField] private int _goldAmountToEarn = 20;


    private Coroutine invulnerabilityCoroutine;

    private bool _firstFrame = false;





    private void Update()
    {
        //Absolutely disgusting. Will have to rework my CharacterStat package in order to take this into account - which happens I DON'T KNOW WHY
        //Probably has to do with initialization steps, although still doesn't work with Enable, Disable, Start and the rest. Insane.

        if (!_firstFrame) {
            _firstFrame = true;
            Health.CurrentValueReachedZero += OnHealthReachedZero;
        }
    }

    public void Hurt(float damage)
    {
        if (_isDead)
        {
            Debug.Log("I'M DEAD LMFAO");
        }
        if (_isInvulnerable)
        {
            Debug.Log("I'M INVULNERABLE LMFAO");
        }
        if (_isDead || _isInvulnerable)
            return;
        Health.Damage(damage);

        CharacterHurt?.Invoke(this);
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

        CharacterHealed?.Invoke(this);
    }

    public void Revive()
    {
        Health.HealToMaxValue();
        _isDead = false;

        CharacterRevived?.Invoke(this);
    }

    private void OnHealthReachedZero()
    {
        if (_isDead) return;
        Debug.LogWarning(this.gameObject.name + " has died!");
        _isDead = true;

        CharacterDied?.Invoke(this);

        // ICI c'est le code de Leezak PAS BEAU MAIS FONCTIONNE
        if (_typeOfCharacter != TypeOfCharacter.Player)
            Player.Instance._currencyScript.AddGold(_goldAmountToEarn);
    }
}
