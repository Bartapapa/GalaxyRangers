using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SC_HubShop : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _blueTokenAmount;
    [SerializeField] private TextMeshProUGUI _relicsAmount;
    [SerializeField] private TextMeshProUGUI _levelAmount;
    [SerializeField] private bool _DebugRecalculateValues = false;

    private void OnEnable()
    {
        ValuesUpdating();
    }

    private void ValuesUpdating()
    {
        // Currency
        _blueTokenAmount.text = Player.Instance._currencyScript.BlueTokenAmount.ToString();
        _relicsAmount.text = Player.Instance._currencyScript.RelicsAmount.ToString();

        // Level
        _levelAmount.text = "LVL " + Player.Instance._currencyScript.current_XPLevelAmount.ToString();

        Debug.Log("Mise a jour des valeurs de la boutique de relation");
    }



    private void Update()
    {
        if (_DebugRecalculateValues)
        {
            ValuesUpdating();
            _DebugRecalculateValues = false;
        }
    }
}
