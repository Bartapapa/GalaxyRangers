using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SC_UIAbility : MonoBehaviour
{
    [SerializeField] private Slider _sliderAbility;

    void Update()
    {
        _sliderAbility.value = Player.Instance._specialityRef_1._capacityLoadValue;
    }
}
