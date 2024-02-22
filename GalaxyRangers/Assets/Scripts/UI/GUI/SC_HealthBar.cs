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
    private bool _inverseAnimationToHeal = false;


    [SerializeField] private bool _Inverse_isUsed = false;
    [SerializeField] private float _InverseDurationTimer = 5.0f;
    private float _InverseCooldownTimer = 0.0f;
    [SerializeField] private Color _ColorOriginal = new Color(1.0f, 1.0f, 1.0f, 1.0f);

    private void Start() {
        ch_health = Player.Instance.CharacterHealth;
        _ColorOriginal = easeHealthSlider.fillRect.GetComponent<Image>().color;
    }
    
    private void Update()
    {
        if (_Inverse_isUsed == true)
        {
            _InverseCooldownTimer += Time.deltaTime;
            if (_InverseCooldownTimer >= _InverseDurationTimer)
            {
                _Inverse_isUsed = false;
                _InverseCooldownTimer = 0.0f;
                _inverseAnimationToHeal = false;
                easeHealthSlider.fillRect.GetComponent<Image>().color = _ColorOriginal;
            }
        }
        if (_inverseAnimationToHeal == false) {
            if (healthSlider.value != ch_health.Health.CurrentValue)
                healthSlider.value = ch_health.Health.CurrentValue;
            if (ch_health.Health.CurrentValue != easeHealthSlider.value)
                easeHealthSlider.value = Mathf.Lerp(easeHealthSlider.value, ch_health.Health.CurrentValue, lerpSpeed);
            healthSlider.maxValue = ch_health.Health.MaxValue;
            easeHealthSlider.maxValue = ch_health.Health.MaxValue;
            _textCurrentHealth.text = ch_health.Health.CurrentValue.ToString();
            _textMaxHealth.text = ch_health.Health.MaxValue.ToString();
        }
        else {
            if (easeHealthSlider.value != ch_health.Health.CurrentValue)
                easeHealthSlider.value = ch_health.Health.CurrentValue;
            if (ch_health.Health.CurrentValue != healthSlider.value)
                healthSlider.value = Mathf.Lerp(healthSlider.value, ch_health.Health.CurrentValue, lerpSpeed);
            healthSlider.maxValue = ch_health.Health.MaxValue;
            easeHealthSlider.maxValue = ch_health.Health.MaxValue;
            _textCurrentHealth.text = ch_health.Health.CurrentValue.ToString();
            _textMaxHealth.text = ch_health.Health.MaxValue.ToString();
        }
        // if (Input.GetKeyDown(KeyCode.Space))
            // TakeDamage(1);
    }

    public void InverseAnimationToHeal()
    {
        _inverseAnimationToHeal = false;
        _Inverse_isUsed = true;
        // Change the color to green
        easeHealthSlider.fillRect.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    }

    private void TakeDamage(float damage) {
        // ch_health.Hurt(damage);
    }
}
