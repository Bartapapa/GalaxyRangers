using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SC_ItemCaseHubShop : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textPrice = null;
    [SerializeField] private TextMeshProUGUI _textRelationLevelToUnlock = null;
    [SerializeField] private GameObject _imageBlueToken = null;
    [SerializeField] private GameObject _imageRelics = null;
    [SerializeField] private GameObject _Lock_go = null;
    [SerializeField] private GameObject _Unlock_go = null;
    [SerializeField] private GameObject _ItemBuy = null;

    [Header("Item/Ability Infos")]
    [SerializeField] private bool _isBlueToken = false;
    [SerializeField] private int _relationLevelToUnlock = 10;
    [SerializeField] private int _itemPrice = 10;
    [SerializeField] private bool _is_Bought = false;
    [SerializeField] private bool _DebugRefresh = false;

    private void OnEnable()
    {
        Item_Updating();
    }

    private void Item_Updating()
    {
        if (_relationLevelToUnlock <= Player.Instance._currencyScript.current_XPLevelAmount)
        {
            _Lock_go.SetActive(false);
            _Unlock_go.SetActive(true);
            _textPrice.text = _itemPrice.ToString();
            if (_isBlueToken)
            {
                _imageBlueToken.SetActive(true);
                _imageRelics.SetActive(false);
            }
            else
            {
                _imageBlueToken.SetActive(false);
                _imageRelics.SetActive(true);
            }
        }
        else
        {
            _Lock_go.SetActive(true);
            _Unlock_go.SetActive(false);
            _textRelationLevelToUnlock.text = "LVL " + _relationLevelToUnlock.ToString();
        }
    }

    public void TryToBuyItem()
    {
        if (_is_Bought == false) { 
            if (_isBlueToken) {
                if (Player.Instance._currencyScript.BlueTokenAmount >= _itemPrice) {
                    Player.Instance._currencyScript.BlueTokenAmount -= _itemPrice;
                    
                    _is_Bought = true;
                }
            }
            else {
                if (Player.Instance._currencyScript.RelicsAmount >= _itemPrice) {
                    Player.Instance._currencyScript.RelicsAmount -= _itemPrice;

                    _is_Bought = true;
                }
            }
        }
    }
    private void Update()
    {
        if (_DebugRefresh)
        {
            _DebugRefresh = false;
            Item_Updating();
        }
    }
}
