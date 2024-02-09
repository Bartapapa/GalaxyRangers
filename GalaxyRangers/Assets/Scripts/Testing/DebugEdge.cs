using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugEdge : MonoBehaviour
{
    [SerializeField] private Transform _parentNode;
    [SerializeField] private Transform _childNode;
    public Transform ParentNode => _parentNode;
    public Transform ChildNode => _childNode;

    private LineRenderer _line;

    public void BuildLine(Transform parent, Transform child)
    {
        _line = GetComponent<LineRenderer>();
        _parentNode = parent;
        _childNode = child;

        transform.position = parent.transform.position;
        _line.SetPosition(0, parent.transform.position);
        _line.SetPosition(1, child.transform.position);
    }

    public void BuildTeleport(Transform parent, Transform child)
    {
        _line = GetComponent<LineRenderer>();

        _line.startColor = Color.green;
        _line.endColor = Color.green;

        _parentNode = parent;
        _childNode = child;

        transform.position = parent.transform.position + Vector3.up;
        _line.SetPosition(0, parent.transform.position + Vector3.up);
        _line.SetPosition(1, child.transform.position + Vector3.up);
    }


}
