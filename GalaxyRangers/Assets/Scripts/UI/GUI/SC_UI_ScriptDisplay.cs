using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SC_UI_ScriptDisplay : MonoBehaviour
{
    // Variable qui change durant la run
    [Header("")]

    [SerializeField]
    private TextMeshProUGUI goldText;
    
    [SerializeField] private TextMeshProUGUI _currentKillCount_Quest_1;
    [SerializeField] private TextMeshProUGUI _currentKillCount_Quest_2;

    [Header("Quest Variables")]
    // Je sais pas comment faire autrement ^^
    [SerializeField] private TextMeshProUGUI _txtObjective_1 = null;
    [SerializeField] private TextMeshProUGUI _txtObjective_2 = null;
    [SerializeField] private TextMeshProUGUI _txtNumberCount_1 = null;
    [SerializeField] private TextMeshProUGUI _txtNumberCount_2 = null;
    [SerializeField] private TextMeshProUGUI _rewardAmount = null;
    [SerializeField] private TextMeshProUGUI _reputationAmount = null;
    [SerializeField] private Image _imgBlueToken = null;
    [SerializeField] private Image _imgRelics = null;

    private void Update()
    {
        
        // goldText.text = Player.Instance._currencyScript.GoldAmount.ToString();
        // goldText.text = Player.Instance._currencyScript.BlueTokenAmount.ToString();
        // goldText.text = Player.Instance._currencyScript.RelicsAmount.ToString();
    }



    
    public void InitQuestPanel(QuestVariables _questVariables , string _enemyName_1 , string _enemyName_2)
    {
        ;
        _txtObjective_1.text = "Beat " + _questVariables._enemyNumberToKill_1.ToString() + " " + _enemyName_1;
        if (_questVariables._enemyNumberToKill_2 > 0) {
            _txtObjective_2.text = "Beat " + _questVariables._enemyNumberToKill_2.ToString() + " " + _enemyName_2;
        }
        else {
            _txtObjective_2.text = "";
            _txtNumberCount_2.text = "";
        }
        _txtNumberCount_1.text = " /" + _questVariables._enemyNumberToKill_1.ToString();
        _txtNumberCount_2.text = " /" + _questVariables._enemyNumberToKill_2.ToString();
        _rewardAmount.text = _questVariables._rewardMoney.ToString();
        _reputationAmount.text = _questVariables._reputationXPnumber.ToString();
        if (_questVariables._isBlueToken)
        {
            _imgBlueToken.gameObject.SetActive(true);
            _imgRelics.gameObject.SetActive(false);
        }
        else
        {
            _imgBlueToken.gameObject.SetActive(false);
            _imgRelics.gameObject.SetActive(true);
        }
    }
}
