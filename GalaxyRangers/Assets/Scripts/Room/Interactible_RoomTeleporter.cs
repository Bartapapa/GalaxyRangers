using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactible_RoomTeleporter : Interactible
{
    public Teleporter teleporter;
    public float _activationDistance = 5f;

    [Header("DEBUG")]
    [SerializeField] private MeshRenderer _mat;

    //Cached
    [ReadOnlyInspector] public bool canBeActivated = true;

    //Coroutines
    private Coroutine teleportCoroutine;

    public void ActivateTeleporter()
    {
        if (!canBeActivated)
            return;

        _canBeInteractedWith = true;

        if (_mat)
        {
            _mat.material.color = Color.green;
        }
    }

    public void DeactivateTeleporter()
    {
        _canBeInteractedWith = false;

        if (_mat)
        {
            _mat.material.color = Color.red;
        }
    }

    protected override void InteractEvent(InteractibleManager manager)
    {
        //Force MoveToRoom to room determined by teleporter.
        WorldManager.Instance.MoveToRoom(teleporter.toRoom);
        EndInteract(manager);
    }

    private IEnumerator CoTeleport()
    {
        yield return null;
        //End interact.
    }
}
