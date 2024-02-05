using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FocusObject
{
    public Transform Object;
    public int Weight;
    public bool IgnoreZ = true;

    public FocusObject(Transform focusObject, int weight, bool ignoreZ = true)
    {
        Object = focusObject;
        Weight = weight;
        IgnoreZ = ignoreZ;
    }
}

public class PlayerCamera : MonoBehaviour
{
    public CinemachineVirtualCamera Camera;
    [SerializeField] private Transform _focus;
    [SerializeField] private List<FocusObject> _focusObjects = new List<FocusObject>();

    private void Awake()
    {
        if (_focus != null)
        {
            Camera.Follow = _focus;
        }
    }
    private void LateUpdate()
    {
        if (_focus != null && _focusObjects.Count > 0)
        {
            HandleFocus();
        }
    }

    private void HandleFocus()
    {
        if (_focusObjects.Count == 1)
        {
            //Singular object
            Vector3 toPos = _focusObjects[0].IgnoreZ ?
                            new Vector3(_focusObjects[0].Object.position.x, _focusObjects[0].Object.position.y, 0f) :
                            _focusObjects[0].Object.position;
            _focus.position = toPos;
        }
        else
        {
            //Multiple objects
            Vector3 averagePos = Vector3.zero;
            foreach(FocusObject focusObject in _focusObjects)
            {
                Vector3 focusObjectPos = focusObject.IgnoreZ ?
                            new Vector3(focusObject.Object.position.x, focusObject.Object.position.y, 0f) :
                            focusObject.Object.position;
                averagePos += (focusObjectPos * focusObject.Weight);
            }
            averagePos = averagePos / _focusObjects.Count;
            _focus.position = averagePos;
        }
    }

    public void AddFocusObject(Transform focusObject, int weight = 1, bool ignoreZ = true)
    {
        bool isPresentInList = false;
        foreach(FocusObject listedObject in _focusObjects)
        {
            if (listedObject.Object == focusObject)
            {
                isPresentInList = true;
                break;
            }
        }

        if (!isPresentInList)
        {
            _focusObjects.Add(new FocusObject(focusObject, weight, ignoreZ));
        }
    }

    public void RemoveFocusObject(Transform focusObject)
    {
        FocusObject chosenFO = null;
        foreach (FocusObject listedObject in _focusObjects)
        {
            if (listedObject.Object == focusObject)
            {
                chosenFO = listedObject;
                break;
            }
        }

        if (chosenFO != null)
        {
            _focusObjects.Remove(chosenFO);
        }
    }

    public void ClearFocusObjects()
    {
        _focusObjects.Clear();
    }

    public void ForceFocusToPosition(Vector3 position)
    {
        Vector3 focusPos = _focus.position;
        Vector3 delta = focusPos - position;
        _focus.position = position;
        Camera.OnTargetObjectWarped(_focus, delta);
    }
}
