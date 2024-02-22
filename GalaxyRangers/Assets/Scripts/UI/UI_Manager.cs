using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{
    public static UI_Manager Instance;

    [Header("UI References")]
    [SerializeField] private GameObject _HubShopRef;
    [SerializeField] private GameObject _HubShop;
    [SerializeField] private GameObject _HubQuestRef;
    public SC_HealthBar _HealthBarScript;
    [SerializeField] private Animator _HubButtonshop;
    [SerializeField] private Animator _HubButtonQuest;
    [SerializeField] private GameObject _GUIRef;
    [SerializeField] private SC_UI_ScriptDisplay _scriptDisplay;
    [SerializeField] public SC_PauseMenu _scriptPauseMenu;

    [Header("Boss")]
    private bool _roomOfBoss;
    public Slider healthSliderBOSS;
    public Slider easeHealthSliderBOSS;
    private CharacterHealth _scriptCharBossHealth;
    private float lerpSpeedBOSS = 0.025f;
    [SerializeField] private GameObject _bossHealthBar;


    public SC_UI_ScriptDisplay _scriptDisplayRef { get { return _scriptDisplay; } }


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

    public void EnterInBossRoom(CharacterHealth _scriptCharaBossHealth)
    {
        _scriptCharBossHealth = _scriptCharaBossHealth;
        _roomOfBoss = true;
        _bossHealthBar.SetActive(true);
    }

    public void ExitBossRoom()
    {
        _roomOfBoss = false;
        _bossHealthBar.SetActive(false);
    }


    private void Update()
    {
        if (_roomOfBoss)
        {
            UpdateBossHealth();
        }
    }

    private void UpdateBossHealth()
    {
        if (healthSliderBOSS.value != _scriptCharBossHealth.Health.CurrentValue)
            healthSliderBOSS.value = _scriptCharBossHealth.Health.CurrentValue;
        if (_scriptCharBossHealth.Health.CurrentValue != easeHealthSliderBOSS.value)
            easeHealthSliderBOSS.value = Mathf.Lerp(easeHealthSliderBOSS.value, _scriptCharBossHealth.Health.CurrentValue, lerpSpeedBOSS);
        healthSliderBOSS.maxValue = _scriptCharBossHealth.Health.MaxValue;
        easeHealthSliderBOSS.maxValue = _scriptCharBossHealth.Health.MaxValue;
    }

    public void OpenHubShop()
    {
        _HubShopRef.SetActive(true);
        _HubButtonshop.SetTrigger("Click");
        _GUIRef.SetActive(false);
        Time.timeScale = 0;
    }

    public void CloseHubShop()
    {
        _HubQuestRef.SetActive(false);
        _HubButtonQuest.SetTrigger("Unclick");
        _HubShop.SetActive(true);
        _HubShopRef.SetActive(false);
        _GUIRef.SetActive(true);
        Time.timeScale = 1;
    }
}
