using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugEdge : MonoBehaviour
{
    [SerializeField] private DebugRoom _parentRoom;
    [SerializeField] private DebugRoom _childRoom;
    public DebugRoom ParentNode => _parentRoom;
    public DebugRoom ChildNode => _childRoom;

    private LineRenderer _line;

    public void BuildLine(DebugRoom parent, DebugRoom child)
    {
        _line = GetComponent<LineRenderer>();
        _parentRoom = parent;
        _childRoom = child;

        transform.position = parent.transform.position;
        _line.SetPosition(0, parent.transform.position);
        _line.SetPosition(1, child.transform.position);
    }


}
