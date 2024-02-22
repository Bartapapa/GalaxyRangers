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
    // [SerializeField] private GameObject _ItemBuy = null;

    [Header("Item/Ability Infos")]
    [SerializeField] private bool _isBlueToken = false;
    [SerializeField] private int _relationLevelToUnlock = 10;
    [SerializeField] private int _itemPrice = 10;
    [SerializeField] private bool _is_Bought = false;
    [SerializeField] private bool _DebugRefresh = false;

    [Header("Weapon ref")]
    [SerializeField] private Weapon _sword;
    [SerializeField] private bool _IsdoOnce = false;
    private bool _Activated = false;
    private int xp = 0;

    private void OnEnable()
    {
        Item_Updating();
    }

    public void Item_Updating()
    {
        xp = Player.Instance._currencyScript.current_XPLevelAmount;
        if (_Activated)
        {
            xp = Player.Instance._currencyScript.current_XPLevelAmount + 1;
        }
        if (_IsdoOnce)
        {
            _Activated = true;
        }

        if (_relationLevelToUnlock <= xp)
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
        if (_is_Bought)
        {
            _textPrice.text = "";
            _imageBlueToken.SetActive(false);
            _imageRelics.SetActive(false);
        }
    }

    public void TryToBuyItem()
    {
        if (_is_Bought == false) { 
            if (_isBlueToken) {
                if (Player.Instance._currencyScript.BlueTokenAmount >= _itemPrice){
                    Player.Instance._currencyScript.BlueTokenAmount -= _itemPrice;


                    _is_Bought = true;

                    Player.Instance.CharacterCombat.EquipWeapon(_sword);
                    //Should work
                }
            }
            else {
                if (Player.Instance._currencyScript.RelicsAmount >= _itemPrice) {
                    Player.Instance._currencyScript.RelicsAmount -= _itemPrice;
                    UI_Manager.Instance._textCurrency.text = Player.Instance._currencyScript.RelicsAmount.ToString();

                    _is_Bought = true;
                    Item_Updating();

                    Player.Instance.CharacterCombat.EquipWeapon(_sword);
                    //Should work
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
