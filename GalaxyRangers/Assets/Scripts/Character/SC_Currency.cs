using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_Currency : MonoBehaviour
{
    [Header("Currency")]
    // Code pas beau de Leezak
    // J'ai mis en public pour faire des debugs j'ai quand meme fais des fonctions de maniere plus propre si jamais
    // Surtout pour la prese on va surement modifier la currency pour notre mort
    public int GoldAmount = 0;
    public int BlueTokenAmount = 0;
    public int RelicsAmount = 0;
    // Start is called before the first frame update

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
