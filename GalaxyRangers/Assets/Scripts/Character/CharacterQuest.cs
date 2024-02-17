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

    public void InitQuest(QuestVariables _variables)
    {
        _questVariables = _variables;
    }


}
