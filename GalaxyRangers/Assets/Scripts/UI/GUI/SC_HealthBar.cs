using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SC_HealthBar : MonoBehaviour
{

    private CharacterHealth ch_health;

    public Slider healthSlider;
    public Slider easeHealthSlider;
    [SerializeField]
    private TextMeshProUGUI _textCurrentHealth;
    
    [SerializeField]
    private TextMeshProUGUI _textMaxHealth;
    private float lerpSpeed = 0.025f;

    private void Start() {
        ch_health = Player.Instance.CharacterHealth;
    }
    
    private void Update()
    {
        if (healthSlider.value != ch_health.Health.CurrentValue)
            healthSlider.value = ch_health.Health.CurrentValue;
        if (ch_health.Health.CurrentValue != easeHealthSlider.value)
            easeHealthSlider.value = Mathf.Lerp(easeHealthSlider.value, ch_health.Health.CurrentValue, lerpSpeed);
        healthSlider.maxValue = ch_health.Health.MaxValue;
        easeHealthSlider.maxValue = ch_health.Health.MaxValue;
        _textCurrentHealth.text = ch_health.Health.CurrentValue.ToString();
        _textMaxHealth.text = ch_health.Health.MaxValue.ToString();
        // if (Input.GetKeyDown(KeyCode.Space))
            // TakeDamage(1);
    }

    private void TakeDamage(float damage) {
        // ch_health.Hurt(damage);
    }
}
