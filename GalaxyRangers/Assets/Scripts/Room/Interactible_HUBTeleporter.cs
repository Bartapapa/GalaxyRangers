using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactible_HUBTeleporter : Interactible
{
    [Header("FORCE SEED")]
    public string forceSeed = "";

    protected override void InteractEvent(InteractibleManager manager)
    {
        //Teleport out of world, generate new one based on seed.
        WorldManager.Instance.StartNewRun(forceSeed);

        EndInteract(manager);
    }
}
