using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SC_HubQuest : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _txtRewardMoney = null;
    [SerializeField] private TextMeshProUGUI _txtReputationNumber = null;
    [SerializeField] private TextMeshProUGUI _txtObjective_1 = null;
    [SerializeField] private Image _imgBlueToken = null;
    [SerializeField] private Image _imgRelics = null;

    [Header("Variables")]
    [Space(5)]

    [SerializeField] private int _rewardMoney = 0;
    [SerializeField] private int _reputationXPnumber = 0;
    [SerializeField] private bool _isBlueToken = false;
    [SerializeField] private TypeOfCharacter _enemyTypeToKill_1 = TypeOfCharacter.EnemyCac;
    [SerializeField] private int _enemyNumberToKill_1 = 0;
    [SerializeField] private TypeOfCharacter _enemyTypeToKill_2 = TypeOfCharacter.EnemyCac;
    [SerializeField] private int _enemyNumberToKill_2 = 0;
    [SerializeField] private bool _isSpecialQuestWithoutBeingTouch = false;
    private QuestVariables _questVariables = new QuestVariables();
    
    private string _tmpEnemy_1;
    private string _tmpEnemy_2;

// Initialize the UI with the quest's data
    private void OnEnable() {
        _txtRewardMoney.text = _rewardMoney.ToString();
        _txtReputationNumber.text = _reputationXPnumber.ToString();

        SetTextTemporar();

        if (_isSpecialQuestWithoutBeingTouch) {
            _txtObjective_1.text = "Kill " + _enemyNumberToKill_1 + " " + _tmpEnemy_1 +
            " without\n being touched";
            _enemyNumberToKill_2 = 0;
        }
        else {
            if (_enemyNumberToKill_2 == 0)
                _txtObjective_1.text = "Kill " + _enemyNumberToKill_1 + " " + _tmpEnemy_1;
            else
                _txtObjective_1.text = "Kill " + _enemyNumberToKill_1 + " " + _tmpEnemy_1 + "\nMurder " + _enemyNumberToKill_2 + " " + _tmpEnemy_2;
        }
        if (_isBlueToken) {
            _imgBlueToken.gameObject.SetActive(true);
            _imgRelics.gameObject.SetActive(false);
        }
        else {
            _imgBlueToken.gameObject.SetActive(false);
            _imgRelics.gameObject.SetActive(true);
        }
    }

// Au moins on peut mettre le nom quon veut dans les variables
    private void SetTextTemporar()
    {
        if (_enemyTypeToKill_1 == TypeOfCharacter.EnemyCac)
            _tmpEnemy_1 = "Basics";
        else if (_enemyTypeToKill_1 == TypeOfCharacter.EnemyCacEpic)
            _tmpEnemy_1 = "Epic";
        else if (_enemyTypeToKill_1 == TypeOfCharacter.EnemyLongRange)
            _tmpEnemy_1 = "Basics distance";
        else if (_enemyTypeToKill_1 == TypeOfCharacter.EnemyLongRangeEpic)
            _tmpEnemy_1 = "Epic distance";
        else if (_enemyTypeToKill_1 == TypeOfCharacter.EnemyBoss)
            _tmpEnemy_1 = "Boss";
        if (_enemyTypeToKill_2 == TypeOfCharacter.EnemyCac)
            _tmpEnemy_2 = "Basics";
        else if (_enemyTypeToKill_2 == TypeOfCharacter.EnemyCacEpic)
            _tmpEnemy_2 = "Epic";
        else if (_enemyTypeToKill_2 == TypeOfCharacter.EnemyLongRange)
            _tmpEnemy_2 = "Basics distance";
        else if (_enemyTypeToKill_2 == TypeOfCharacter.EnemyLongRangeEpic)
            _tmpEnemy_2 = "Epic distance";
        else if (_enemyTypeToKill_2 == TypeOfCharacter.EnemyBoss)
            _tmpEnemy_2 = "Boss";
    }

    private void InitStructure()
    {
        _questVariables._enemyTypeToKill_1 = _enemyTypeToKill_1;
        _questVariables._enemyNumberToKill_1 = _enemyNumberToKill_1;

        _questVariables._enemyTypeToKill_2 = _enemyTypeToKill_2;
        _questVariables._enemyNumberToKill_2 = _enemyNumberToKill_2;

        _questVariables._isBlueToken = _isBlueToken;
        _questVariables._isSpecialQuestWithoutBeingTouch = _isSpecialQuestWithoutBeingTouch;
        _questVariables._reputationXPnumber = _reputationXPnumber;
        _questVariables._rewardMoney = _rewardMoney;

    }

    public void QuestSelectedConfirmed()
    {
        // Init the structure of CharacterQuest With my variables
        InitStructure();
        Player.Instance.CharacterQuest.InitQuest(_questVariables);
        UI_Manager.Instance._scriptDisplayRef.InitQuestPanel(_questVariables, _tmpEnemy_1, _tmpEnemy_2);
    }

}
