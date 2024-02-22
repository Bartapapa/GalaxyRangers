using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractibleShopRunStatut : Interactible
{
    private enum StatutType
    {
        Health,
        Damage,
        DashDistance,
        Speed
    }
    [SerializeField] private StatutType _statutType;
    [SerializeField] private TextMeshProUGUI _textPrice;
    [SerializeField] private TextMeshProUGUI _textValue;
    [SerializeField] private TextMeshProUGUI _textGlobal;
    [SerializeField] private TextMeshProUGUI _txt_Title;
    [SerializeField] private TextMeshProUGUI _txt_TitleLevel;


    [SerializeField] private GameObject _shopPanel;
    [SerializeField] private int price = 10;
    [SerializeField] private int _currentLevel = 0;
    
    [SerializeField] private GameObject _lifeGO;
    [SerializeField] private GameObject _damageGO;
    [SerializeField] private GameObject _dashDistanceGO;
    [SerializeField] private GameObject _speedGO;

    [SerializeField] private GameObject _crossGO;



    [SerializeField] private List<int> _PriceByLevel_Health = new List<int>() { 10, 20, 30, 40};
    [SerializeField] private List<float> _ValueByLevel_Health = new List<float>() { 10, 20, 30, 40};
    [SerializeField] private List<int> _PriceByLevel_Damage = new List<int>() { 10, 20, 30, 40};
    [SerializeField] private List<float> _ValueByLevel_Damage = new List<float>() { 10, 20, 30, 40};
    [SerializeField] private List<int> _PriceByLevel_DashDistance = new List<int>() { 10, 20, 30, 40};
    [SerializeField] private List<float> _ValueByLevel_DashDistance = new List<float>() { 10, 20, 30, 40};
    [SerializeField] private List<int> _PriceByLevel_Speed = new List<int>() { 10, 20, 30, 40};
    [SerializeField] private List<float> _ValueByLevel_Speed = new List<float>() { 10, 20, 30, 40};

    protected override void InteractEvent(InteractibleManager manager)
    {
        TryToBuyItem();
        EndInteract(manager);
    }

    private void Start()
    {
        switch (_statutType)
        {
           case StatutType.Health:
               price = _PriceByLevel_Health[_currentLevel];
                _txt_Title.text = "HEALTH";
                _txt_TitleLevel.text = "LVL " + (_currentLevel + 1);
               _textGlobal.text = "YOUR HEALTH IS INCREASE BY";
               _textValue.text = "+" + _ValueByLevel_Health[_currentLevel].ToString() + "%";
               _textPrice.text = _PriceByLevel_Health[_currentLevel].ToString();
               _lifeGO.SetActive(true);
               break;
           case StatutType.Damage:
                price = _PriceByLevel_Damage[_currentLevel];
                _txt_Title.text = "DAMAGE";
                _txt_TitleLevel.text = "LVL " + (_currentLevel + 1);
                _textGlobal.text = "YOUR DAMAGE IS INCREASE BY";
                _textValue.text = "+" + _ValueByLevel_Damage[_currentLevel].ToString() + "%";
                _textPrice.text = _PriceByLevel_Damage[_currentLevel].ToString();
                _damageGO.SetActive(true);
               break;
           case StatutType.DashDistance:
                price = _PriceByLevel_DashDistance[_currentLevel];
                _txt_Title.text = "DASH";
                _txt_TitleLevel.text = "LVL " + (_currentLevel + 1);
                _textGlobal.text = "YOUR DASH DISTANCE IS\nINCREASE BY";
                _textValue.text = "+" + _ValueByLevel_DashDistance[_currentLevel].ToString() + "%";
                _textPrice.text = _PriceByLevel_DashDistance[_currentLevel].ToString();
                _dashDistanceGO.SetActive(true);
               break;
           case StatutType.Speed:
                price = _PriceByLevel_Speed[_currentLevel];
                _txt_Title.text = "SPEED";
                _txt_TitleLevel.text = "LVL " + (_currentLevel + 1);
                _textGlobal.text = "YOUR SPEED IS INCREASE BY";
                _textValue.text = "+" + _ValueByLevel_Speed[_currentLevel].ToString() + "%";
                _textPrice.text = _PriceByLevel_Speed[_currentLevel].ToString();
                _speedGO.SetActive(true);
               break;
        }        
    }

    private void Update() {
        if (Player.Instance._currencyScript.GoldAmount < price)
            _crossGO.SetActive(true);
        else
            _crossGO.SetActive(false);
    }

    private void TryToBuyItem()
    {
        if (Player.Instance._currencyScript.GoldAmount >= price)
        {
            Player.Instance._currencyScript.AddGold(-price);
            _shopPanel.SetActive(false);
            Debug.LogWarning("Augmenter level dans un manager: ca");
            // _currentLevel;

            // Need to add the statut to the player
            switch (_statutType) {
                case StatutType.Health:
                    break;
                case StatutType.Damage:
                    break;
                case StatutType.DashDistance:
                    break;
                case StatutType.Speed:
                    break;
            }
            
            Destroy(gameObject);

        }
    }

    public override void SelectInteractible()
    {
        _shopPanel.SetActive(true);
        _textPrice.text = price.ToString();
    }

    public override void DeselectInteractible()
    {
        _shopPanel.SetActive(false);
    }
}
