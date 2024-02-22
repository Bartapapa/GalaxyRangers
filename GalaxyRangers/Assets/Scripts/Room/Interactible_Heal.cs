using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactible_Heal : Interactible
{
    [SerializeField] private int _healPurcentage = 35;
    private float _totalLife = 0;

    [SerializeField] private GameObject HeartGO = null;


    public override void SelectInteractible()
    {
        UI_Manager.Instance._HealthBarScript.InverseAnimationToHeal();
        
        _totalLife = (Player.Instance.CharacterHealth.MaxHealth * _healPurcentage) / 100;
        Player.Instance.CharacterHealth.Heal(_totalLife);
        Destroy(this.gameObject);
    }

    public override void DeselectInteractible()
    {

    }

    private void Update()
    {
        HeartGO.transform.Rotate(0, 0, 0.4f);
    }

}
