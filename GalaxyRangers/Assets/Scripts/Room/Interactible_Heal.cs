using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactible_Heal : Interactible
{
    [SerializeField] private int _healAmount = 200;

    public override void SelectInteractible()
    {
        UI_Manager.Instance._HealthBarScript.InverseAnimationToHeal();
        Player.Instance.CharacterHealth.Heal(_healAmount);
        Destroy(this.gameObject);
    }

    public override void DeselectInteractible()
    {

    }

}
