using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Manager : MonoBehaviour
{
    public static UI_Manager Instance;

    [Header("UI References")]
    [SerializeField] private GameObject _HubShopRef;
    [SerializeField] private GameObject _GUIRef;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("2 or more UIMANAGER found. Removing the latest ones.");
            Destroy(this.gameObject);
        }
    }

    public void OpenHubShop()
    {
        _HubShopRef.SetActive(true);
        _GUIRef.SetActive(false);
        Time.timeScale = 0;
    }

    public void CloseHubShop()
    {
        _HubShopRef.SetActive(false);
        _GUIRef.SetActive(true);
        Time.timeScale = 1;
    }
}
