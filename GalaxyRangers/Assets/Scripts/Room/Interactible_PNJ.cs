using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Interactible_PNJ : Interactible
{
    [SerializeField] private GameObject _interactPanel;
    public override void SelectInteractible()
    {
        _interactPanel.SetActive(true);
    }

    public override void DeselectInteractible()
    {
        _interactPanel.SetActive(false);
    }

    protected override void InteractEvent(InteractibleManager manager)
    {
        // Debug.Log("Talk to the PNJ");
        UI_Manager.Instance.OpenHubShop();
        EndInteract(manager);
    }
}
