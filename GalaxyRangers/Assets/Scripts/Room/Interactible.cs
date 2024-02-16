using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactible : MonoBehaviour
{
    [Header("INTERACTION PROMPT")]
    private float whee;

    [Header("INTERACTION PARAMETERS")]
    protected bool _canBeInteractedWith = true;
    public bool canBeInteractedWith { get { return _canBeInteractedWith; } }

    //Cached
    protected bool _isBeingInteractedWith = false;
    protected InteractibleManager _currentManager = null;


    public virtual void SelectInteractible()
    {
        //Visual feedback of selected interactible.
    }

    public virtual void DeselectInteractible()
    {
        //Visual feedback of deselected interactible.
    }

    public void StartInteract(InteractibleManager manager)
    {
        _isBeingInteractedWith = true;
        _canBeInteractedWith = false;
        _currentManager = manager;
        InteractEvent(_currentManager);
    }

    protected virtual void InteractEvent(InteractibleManager manager)
    {
        //What does the interactible do when interacted with?
        EndInteract(manager);
    }

    public virtual void EndInteract(InteractibleManager manager)
    {
        manager.EndInteract();
        manager = null;
        _isBeingInteractedWith = false;
        _canBeInteractedWith = true;
    }
}
