using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SC_UI_ScriptDisplay : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI goldText;
    private void Update()
    {
        // goldText.text = Player.Instance._currencyScript.GoldAmount.ToString();
        // goldText.text = Player.Instance._currencyScript.BlueTokenAmount.ToString();
        // goldText.text = Player.Instance._currencyScript.RelicsAmount.ToString();
    }
}
