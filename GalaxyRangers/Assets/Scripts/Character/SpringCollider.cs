using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringCollider : MonoBehaviour
{
    //??
    public float radius = 0.5f;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
