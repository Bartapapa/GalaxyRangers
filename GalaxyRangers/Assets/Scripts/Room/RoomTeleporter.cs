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

        BaseCharacterController player = other.GetComponent<BaseCharacterController>();
        if (player)
        {
            if (player.faction == GameFaction.Player && !GameManager.Instance.isInFade)
            {
                GameManager.Instance.Fade(.5f,
                    () => WorldManager.Instance.MoveToRoom(_traversalPoint.toRoom, _traversalPoint));
            }
        }
    }
}
