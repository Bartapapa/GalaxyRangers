using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SC_TestUI : MonoBehaviour
{
    public CharacterHealth ch_health;

    public Slider healthSlider;
    public Slider easeHealthSlider;
    // public float maxHealth = 100f;
    // public float health;
    private float lerpSpeed = 0.075f;

    private void OnEnable()
    {
        //ch_health.CharacterHurt -= OnCharacterHurt;
        //ch_health.CharacterHurt += OnCharacterHurt;
    }

    private void OnDisable()
    {
        //ch_health.CharacterHurt -= OnCharacterHurt;
    }

    private void OnCharacterHurt(CharacterHealth characterHealth)
    {
        //ch_health.Health.CurrentValue;
        //ch_health.Health.MaxValue;
    }


    private void Update()
    {
        if (healthSlider.value != ch_health.Health.CurrentValue) {
            healthSlider.value = ch_health.Health.CurrentValue;
        }
        if (Input.GetKeyDown(KeyCode.Space)) {
            TakeDamage(10);
        }
        if (ch_health.Health.CurrentValue != easeHealthSlider.value) {
            easeHealthSlider.value = Mathf.Lerp(easeHealthSlider.value, ch_health.Health.CurrentValue, lerpSpeed);
        }
        healthSlider.maxValue = ch_health.Health.MaxValue;
        easeHealthSlider.maxValue = ch_health.Health.MaxValue;
        // Camera.main;
    }

    private void TakeDamage(float damage)
    {
        // ch_health.Health.CurrentValue -= damage;
    }
}
