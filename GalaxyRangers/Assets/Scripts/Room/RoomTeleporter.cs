using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTeleporter : MonoBehaviour
{
    private TraversalPoint _traversalPoint;
    public TraversalPoint traversalPoint { get { return _traversalPoint; } }

    public void SetTraversalPoint(TraversalPoint traversalPoint)
    {
        _traversalPoint = traversalPoint;
        Collider collider = GetComponent<Collider>();
        collider.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_traversalPoint == null)
            return;

        PlayerCharacterController player = other.GetComponent<PlayerCharacterController>();
        if (player && !GameManager.Instance.isInFade)
        {
            //Use worldmanager to fade out, change room, fade in.
            //WorldManager.Instance.world.rooms.Find(_traversalPoint.toRoom);
            WorldManager.Instance.MoveToRoom(_traversalPoint.toRoom, _traversalPoint);
        }
    }
}
