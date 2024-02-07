using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Graph
{
    [SerializeField] [HideInInspector] protected List<Node> _nodes = new List<Node>();
    public List<Node> nodes { get { return _nodes; } }

    protected Queue<Node> _nodeQueue = new Queue<Node>();
    protected Stack<Node> _nodeStack = new Stack<Node>();

    protected void BFS()
    {

    }

    protected void DFS()
    {

    }
}
