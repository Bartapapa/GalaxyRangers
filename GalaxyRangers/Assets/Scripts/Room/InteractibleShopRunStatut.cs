using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractibleShopRunStatut : Interactible
{
    [SerializeField] private TextMeshProUGUI _textPrice;
    [SerializeField] private GameObject _shopPanel;
    [SerializeField] private int price = 10;
    protected override void InteractEvent(InteractibleManager manager)
    {
        TryToBuyItem();
        EndInteract(manager);
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
