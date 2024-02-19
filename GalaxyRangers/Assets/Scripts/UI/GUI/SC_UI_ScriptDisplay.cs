using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SC_UI_ScriptDisplay : MonoBehaviour
{
    // Variable qui change durant la run
    [Header("")]

    [SerializeField] private TextMeshProUGUI txt_goldText;
    [SerializeField] private TextMeshProUGUI txt_blueToken;
    [SerializeField] private TextMeshProUGUI txt_relics;
    
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
    [Header("Quest Display")]
    [SerializeField] private GameObject _questPanel = null;
    [SerializeField] private GameObject _questLogo = null;
    private bool _questPanelActive = true;


    

    private void Update()
    {
        
        txt_goldText.text = Player.Instance._currencyScript.GoldAmount.ToString();
        txt_blueToken.text = Player.Instance._currencyScript.BlueTokenAmount.ToString();
        txt_relics.text = Player.Instance._currencyScript.RelicsAmount.ToString();
    }

    public void ChangeValueCountKill(int _currentKill_1 , int _currentKill_2)
    {
        _currentKillCount_Quest_1.text = _currentKill_1.ToString();
        _currentKillCount_Quest_2.text = _currentKill_2.ToString();
    }

    public void DisplayQuestPanel()
    {
        if (_questPanelActive == true) {
            _questPanelActive = false;
            _questPanel.SetActive(false);
            _questLogo.gameObject.SetActive(true);
        }
        else {
            _questPanelActive = true;
            _questPanel.SetActive(true);
            _questLogo.gameObject.SetActive(false);
        }
    }


    
    public void InitQuestPanel(QuestVariables _questVariables , string _enemyName_1 , string _enemyName_2)
    {
        ChangeValueCountKill(0, 0);
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
