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

    [SerializeField] private GameObject _shopPanel;
    [SerializeField] private int price = 10;
    [SerializeField] private int _currentLevel = 0;

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

    private void OnEnable()
    {
        //switch (_statutType)
        //{
        //    case StatutType.Health:
        //        _textValue.text = _ValueByLevel_Health[_currentLevel].ToString();
        //        _textPrice.text = _ValueByLevel_Health[_currentLevel].ToString();
        //        price = _PriceByLevel_Health[_currentLevel];
        //        break;
        //    case StatutType.Damage:
        //        _textValue.text = _ValueByLevel_Damage[_currentLevel].ToString();
        //        _textPrice.text = _ValueByLevel_Damage[_currentLevel].ToString();
        //        price = _PriceByLevel_Damage[_currentLevel];
        //        break;
        //    case StatutType.DashDistance:
        //        _textValue.text = _ValueByLevel_DashDistance[_currentLevel].ToString();
        //        _textPrice.text = _ValueByLevel_DashDistance[_currentLevel].ToString();
        //        price = _PriceByLevel_DashDistance[_currentLevel];
        //        break;
        //    case StatutType.Speed:
        //        _textValue.text = _ValueByLevel_Speed[_currentLevel].ToString();
        //        _textPrice.text = _ValueByLevel_Speed[_currentLevel].ToString();
        //        price = _PriceByLevel_Speed[_currentLevel];
        //        break;
        //}
    }

    private void TryToBuyItem()
    {
        if (Player.Instance._currencyScript.GoldAmount >= price)
        {
            Player.Instance._currencyScript.AddGold(-price);
            _shopPanel.SetActive(false);

            // Need to add the statut to the player
            
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
