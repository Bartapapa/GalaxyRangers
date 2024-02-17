using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_Currency_Relation : MonoBehaviour
{
    [Header("Currency at start")]
    // Code pas beau de Leezak
    // J'ai mis en public pour faire des debugs j'ai quand meme fais des fonctions de maniere plus propre si jamais
    // Surtout pour la prese on va surement modifier la currency pour notre mort
    public int GoldAmount = 0;
    public int BlueTokenAmount = 0;
    public int RelicsAmount = 0;
    public int current_XPAmount = 10;
    public int current_XPLevelAmount = 1;

    [Header("Do not touch")]
    public bool NewXP_Relationship = false;
    public int New_XPAmount = 0;

    public void AddGold(int _gold)
    {
        GoldAmount += _gold;
    }

    public void AddBlueToken(int _blueToken)
    {
        BlueTokenAmount += _blueToken;
    }

    public void AddRelics(int _relics)
    {
        RelicsAmount += _relics;
    }
}
