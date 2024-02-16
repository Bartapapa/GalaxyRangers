using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Interactible_PNJ : Interactible
{
    protected override void InteractEvent(InteractibleManager manager)
    {
        Debug.Log("Talk to the PNJ");

        EndInteract(manager);
    }
}
