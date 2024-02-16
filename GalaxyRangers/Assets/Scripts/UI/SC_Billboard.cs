using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_Billboard : MonoBehaviour
{
    private void LateUpdate()
    {
        transform.LookAt(transform.position + Camera.main.gameObject.transform.forward);
    }
}