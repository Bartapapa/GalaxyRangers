using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct QuestVariables
{
    public int _rewardMoney;
    public int _reputationXPnumber;
    public bool _isBlueToken;
    public TypeOfCharacter _enemyTypeToKill_1;
    public int _enemyNumberToKill_1;
    public TypeOfCharacter _enemyTypeToKill_2;
    public int _enemyNumberToKill_2;
    public bool _isSpecialQuestWithoutBeingTouch;
}


public class CharacterQuest : MonoBehaviour
{
    public QuestVariables _questVariables;
    public bool _isQuestCompleted = false;
    public bool _isQuestActive = false;
    private List<int> _numberofKill = new List<int>();
    private List<TypeOfCharacter> typeOfCharacters = new List<TypeOfCharacter>();
    private int _currentNbKill_1 = 0;
    private int _currentNbKill_2 = 0;


    private void Start()
    {
        typeOfCharacters.Add(TypeOfCharacter.EnemyCac);
        _numberofKill.Add(0);
        typeOfCharacters.Add(TypeOfCharacter.EnemyCacEpic);
        _numberofKill.Add(0);
        typeOfCharacters.Add(TypeOfCharacter.EnemyLongRange);
        _numberofKill.Add(0);
        typeOfCharacters.Add(TypeOfCharacter.EnemyLongRangeEpic);
        _numberofKill.Add(0);
        typeOfCharacters.Add(TypeOfCharacter.EnemyBoss);
        _numberofKill.Add(0);
    }

    public void InitQuest(QuestVariables _variables)
    {
        _questVariables = _variables;
        _currentNbKill_1 = 0;
        _currentNbKill_2 = 0;
        _isQuestActive = true;
    }

    public void SetTheCurrentNbKill(TypeOfCharacter _type)
    {
        for (int i = 0; i < typeOfCharacters.Count; i++)
        {
            if (typeOfCharacters[i] == _type)
                _numberofKill[i]++;
        }
        if (_isQuestActive && !_isQuestCompleted) {
            CheckToIncrQuest(_type);
            UI_Manager.Instance._scriptDisplayRef.ChangeValueCountKill(_currentNbKill_1,_currentNbKill_2);
            CheckIfCompletedQuest();
        }
    }

    public void CheckToIncrQuest(TypeOfCharacter _type)
    {
        switch (_questVariables._enemyTypeToKill_1)
        {
            case TypeOfCharacter.EnemyCac:
                if (_numberofKill[0] >= _questVariables._enemyNumberToKill_1)
                    _currentNbKill_1++;
                break;
            case TypeOfCharacter.EnemyCacEpic:
                if (_numberofKill[1] >= _questVariables._enemyNumberToKill_1)
                    _currentNbKill_1++;
                break;
            case TypeOfCharacter.EnemyLongRange:
                if (_numberofKill[2] >= _questVariables._enemyNumberToKill_1)
                    _currentNbKill_1++;
                break;
            case TypeOfCharacter.EnemyLongRangeEpic:
                if (_numberofKill[3] >= _questVariables._enemyNumberToKill_1)
                    _currentNbKill_1++;
                break;
            case TypeOfCharacter.EnemyBoss:
                if (_numberofKill[4] >= _questVariables._enemyNumberToKill_1)
                    _currentNbKill_1++;
                break;
        }
        switch (_questVariables._enemyTypeToKill_2)
        {
            case TypeOfCharacter.EnemyCac:
                if (_numberofKill[0] >= _questVariables._enemyNumberToKill_2)
                    _currentNbKill_2++;
                break;
            case TypeOfCharacter.EnemyCacEpic:
                if (_numberofKill[1] >= _questVariables._enemyNumberToKill_2)
                    _currentNbKill_2++;
                break;
            case TypeOfCharacter.EnemyLongRange:
                if (_numberofKill[2] >= _questVariables._enemyNumberToKill_2)
                    _currentNbKill_2++;
                break;
            case TypeOfCharacter.EnemyLongRangeEpic:
                if (_numberofKill[3] >= _questVariables._enemyNumberToKill_2)
                    _currentNbKill_2++;
                break;
            case TypeOfCharacter.EnemyBoss:
                if (_numberofKill[4] >= _questVariables._enemyNumberToKill_2)
                    _currentNbKill_2++;
                break;
        }
    }
    private void CheckIfCompletedQuest()
    {
        if (_currentNbKill_1 >= _questVariables._enemyNumberToKill_1)
        {
            if (_currentNbKill_2 >= _questVariables._enemyNumberToKill_2)
            {
                _isQuestCompleted = true;
                UI_Manager.Instance._scriptDisplayRef.DisplayQuestPanel();
            }
        }
    }
}
