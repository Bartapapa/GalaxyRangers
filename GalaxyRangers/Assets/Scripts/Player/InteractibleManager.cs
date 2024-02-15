using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractibleManager : MonoBehaviour
{
    [Header("PARAMETERS")]
    [SerializeField] private float _interactionRadius = 5f;
    private Interactible _currentSelectedInteractible = null;
    private Interactible _currentInteractingInteractible = null;

    private bool _isInteracting = false;

    private void Update()
    {
        HandleCurrentInteractible();
    }

    private void HandleCurrentInteractible()
    {
        Interactible interactible = GetClosestInteractible(AllPotentialInteractibles());
        if (_currentSelectedInteractible != interactible)
        {
            if (_currentSelectedInteractible != null)
            {
                _currentSelectedInteractible.DeselectInteractible();
            }

            _currentSelectedInteractible = interactible;

            if (_currentSelectedInteractible != null)
            {
                _currentSelectedInteractible.SelectInteractible();
            }
        }
    }

    public void InteractWithCurrentInteractible()
    {
        if (_isInteracting || _currentSelectedInteractible == null)
            return;

        _isInteracting = true;

        _currentInteractingInteractible = _currentSelectedInteractible;
        _currentInteractingInteractible.StartInteract(this);
        Debug.Log("Attempted to interact!");
    }

    public void ForceInteractWithInteractible(Interactible interactible)
    {
        if (_isInteracting)
        {
            _currentInteractingInteractible.EndInteract(this);
        }

        _isInteracting = true;

        _currentInteractingInteractible = interactible;
        _currentInteractingInteractible.StartInteract(this);
    }

    public void EndInteract()
    {
        _isInteracting = false;

        _currentInteractingInteractible = null;
    }

    private List<Interactible> AllPotentialInteractibles()
    {
        List<Interactible> allPotentialInteractibles = new List<Interactible>();

        Collider[] coll = Physics.OverlapSphere(transform.position, _interactionRadius);
        foreach(Collider collider in coll)
        {
            Interactible interactible = collider.gameObject.GetComponent<Interactible>();
            if (interactible)
            {
                if (interactible.canBeInteractedWith)
                {
                    allPotentialInteractibles.Add(interactible);
                }
            }
        }

        Debug.Log(allPotentialInteractibles.Count);
        return allPotentialInteractibles;
    }

    private Interactible GetClosestInteractible(List<Interactible> potentialInteractibles)
    {
        if (potentialInteractibles.Count == 0)
            return null;

        Interactible chosenInteractible = null;
        float closestDistance = float.MaxValue;
        foreach(Interactible interactible in potentialInteractibles)
        {
            float distance = Vector3.Distance(transform.position, interactible.transform.position);
            if(distance < closestDistance)
            {
                closestDistance = distance;
                chosenInteractible = interactible;
            }
        }
        return chosenInteractible;
    }
}
