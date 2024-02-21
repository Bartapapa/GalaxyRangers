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
    [SerializeField] private Slider GaugeXP_fill;
    [SerializeField] private Slider GaugeXP_ease;
    [SerializeField] private List<float> _XPMaxbyLevel = new List<float>() { 100, 200, 300};

    [SerializeField] private bool _DebugRecalculateValues = false;
    private bool _ameliorationGauge = false;
    private bool _tmpDoOnce = false;
    private float _tmpValueReste = 0;
    private bool _round2ofGaugeUpgrade = false;
    private float lerpSpeed = 0.005f;

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
        // XP Gauge
        GaugeXP_fill.maxValue = _XPMaxbyLevel[Player.Instance._currencyScript.current_XPLevelAmount - 1];
        GaugeXP_ease.maxValue = _XPMaxbyLevel[Player.Instance._currencyScript.current_XPLevelAmount - 1];

        // Debug.Log("Mise a jour des valeurs de la boutique de relation");
        if (Player.Instance._currencyScript.NewXP_Relationship) {
            Player.Instance._currencyScript.NewXP_Relationship = false;
            GaugeXP_fill.value = Player.Instance._currencyScript.current_XPAmount;
            if ((Player.Instance._currencyScript.current_XPAmount + Player.Instance._currencyScript.New_XPAmount) > GaugeXP_fill.maxValue) {
                GaugeXP_ease.value = GaugeXP_fill.maxValue;
                _round2ofGaugeUpgrade = true;
                _tmpValueReste = (Player.Instance._currencyScript.current_XPAmount + Player.Instance._currencyScript.New_XPAmount) - GaugeXP_fill.maxValue;
            }
            else {
                GaugeXP_ease.value = Player.Instance._currencyScript.current_XPAmount + Player.Instance._currencyScript.New_XPAmount;
                _round2ofGaugeUpgrade = false;
            }
            _ameliorationGauge = true;
        }
    }



    private void Update()
    {
        if (_DebugRecalculateValues)
        {
            ValuesUpdating();
            _DebugRecalculateValues = false;
        }
        if (_ameliorationGauge == true) {
            // if (GaugeXP_fill.value != GaugeXP_ease.value)
            if (GaugeXP_fill.value >= GaugeXP_ease.value - 0.001)
            {
                if (_round2ofGaugeUpgrade)
                {
                    GaugeXP_fill.value = Mathf.Lerp(GaugeXP_fill.value, GaugeXP_ease.value, lerpSpeed * 3);
                    Debug.Log(GaugeXP_fill.value);
                }
                else
                {
                    GaugeXP_fill.value = Mathf.Lerp(GaugeXP_fill.value, GaugeXP_ease.value, lerpSpeed);
                    Debug.Log("Je tourne");
                }
            }
            else if (_round2ofGaugeUpgrade == true) {
                if (_tmpDoOnce == false) {
                    _tmpDoOnce = true;
                    Player.Instance._currencyScript.current_XPLevelAmount++;
                    _levelAmount.text = "LVL " + Player.Instance._currencyScript.current_XPLevelAmount.ToString();

                    GaugeXP_fill.maxValue = _XPMaxbyLevel[Player.Instance._currencyScript.current_XPLevelAmount - 1];
                    GaugeXP_ease.maxValue = _XPMaxbyLevel[Player.Instance._currencyScript.current_XPLevelAmount - 1];

                    GaugeXP_ease.value = _tmpValueReste;
                    GaugeXP_fill.value = 0;
                }

                if (GaugeXP_fill.value != GaugeXP_ease.value) {
                    GaugeXP_fill.value = Mathf.Lerp(GaugeXP_fill.value, GaugeXP_ease.value, lerpSpeed);
                }
                else {
                    _round2ofGaugeUpgrade = false;
                    _tmpDoOnce = false;
                    _ameliorationGauge = false;
                    Player.Instance._currencyScript.New_XPAmount = 0;
                    Player.Instance._currencyScript.current_XPAmount = _tmpValueReste;
                }
            }


        }
    }
}
