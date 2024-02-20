using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SC_EnemyUI : MonoBehaviour
{

    public CharacterHealth ch_health;

    public Slider healthSlider;
    public Slider easeHealthSlider;
    private float lerpSpeed = 0.02f;

    [SerializeField]
    public int amountOfGOld = 20;

    private void Update()
    {
        if (healthSlider.value != ch_health.Health.CurrentValue)
            healthSlider.value = ch_health.Health.CurrentValue;
        if (ch_health.Health.CurrentValue != easeHealthSlider.value)
            easeHealthSlider.value = Mathf.Lerp(easeHealthSlider.value, ch_health.Health.CurrentValue, lerpSpeed);
        healthSlider.maxValue = ch_health.Health.MaxValue;
        easeHealthSlider.maxValue = ch_health.Health.MaxValue;
        // if (Input.GetKeyDown(KeyCode.Space))
        // TakeDamage(1);
    }
}
