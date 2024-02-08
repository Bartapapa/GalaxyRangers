using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    [SerializeField][HideInInspector] protected Node _parentNode;
    public Node parentNode { get { return _parentNode; } }

    [SerializeField][HideInInspector] protected List<Node> _childNodes = new List<Node>();
    public List<Node> childNodes { get { return _childNodes; } }
}
