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
        Debug.Log("TRAVERSED");

        if (_traversalPoint == null)
        {
            Debug.Log("No traversal point!");
            return;
        }

        BaseCharacterController player = other.GetComponent<BaseCharacterController>();
        if (player)
        {
            Debug.Log(GameManager.Instance.isInFade);
            Debug.Log(player.characterHealth.isDead);

            if (player.faction == GameFaction.Player && !GameManager.Instance.isInFade && !player.characterHealth.isDead)
            {
                GameManager.Instance.Fade(.5f, false,
                    () => WorldManager.Instance.MoveToRoom(_traversalPoint.toRoom, _traversalPoint));
            }
        }
        else
        {
            Debug.Log("No player!");
            return;
        }
    }
}
