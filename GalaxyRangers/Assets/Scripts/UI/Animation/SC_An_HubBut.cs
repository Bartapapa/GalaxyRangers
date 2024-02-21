using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_An_HubBut : MonoBehaviour
{
    [SerializeField] private Animator _HubButtonshop;
    [SerializeField] private Animator _HubButtonQuest;
    private bool _isQuestOpen;


    public void ClickOnButShop()
    {
        if (_isQuestOpen == true)
        {
            _isQuestOpen = false;
            _HubButtonQuest.SetTrigger("Unclick");
            _HubButtonshop.SetTrigger("Click");
        }
    }
    public void ClickOnButQuest()
    {
        if (_isQuestOpen == false)
        {
            _isQuestOpen = true;
            _HubButtonshop.SetTrigger("Unclick");
            _HubButtonQuest.SetTrigger("Click");
        }
    }
}
