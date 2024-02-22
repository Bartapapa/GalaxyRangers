using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactible_RoomTeleporter : Interactible
{
    public Teleporter teleporter;
    public float _activationDistance = 5f;

    [Header("DEBUG")]
    [SerializeField] private MeshRenderer _mat;

    [SerializeField] private GameObject _teleporterGOon;
    [SerializeField] private GameObject _teleporterGOoff;
    [SerializeField] private GameObject _interactPanel;

    //Cached
    [ReadOnlyInspector] public bool canBeActivated = true;

    //Coroutines
    private Coroutine teleportCoroutine;

    public void ActivateTeleporter()
    {
        if (!canBeActivated)
            return;

        _canBeInteractedWith = true;

        _teleporterGOoff.SetActive(false);
        _teleporterGOon.SetActive(true);
        // if (_mat)
        // {
        //     _mat.material.color = Color.green;
        // }
    }

    public void DeactivateTeleporter()
    {
        _canBeInteractedWith = false;

        _teleporterGOoff.SetActive(true);
        _teleporterGOon.SetActive(false);

        // if (_mat)
        // {
        //     _mat.material.color = Color.red;
        // }
    }

    protected override void InteractEvent(InteractibleManager manager)
    {
        //Force MoveToRoom to room determined by teleporter.
        Debug.Log(teleporter);
        Debug.Log(teleporter.toRoom);

        if (!GameManager.Instance.isInFade)
        {
            GameManager.Instance.Fade(.5f,
                () => WorldManager.Instance.MoveToRoom(teleporter.toRoom, null, true),
                () => EndInteract(manager));

            //Use worldmanager to fade out, change room, fade in.
            //WorldManager.Instance.world.rooms.Find(_traversalPoint.toRoom);
            //WorldManager.Instance.MoveToRoom(_traversalPoint.toRoom, _traversalPoint);
        }
        else
        {
            EndInteract(manager);
        }
        
    }

    private IEnumerator CoTeleport()
    {
        yield return null;
        //End interact.
    }

    public override void SelectInteractible()
    {
        if (_canBeInteractedWith)
            _interactPanel.SetActive(true);
    }

    public override void DeselectInteractible()
    {
        if (_canBeInteractedWith)
            _interactPanel.SetActive(false);
    }
}
