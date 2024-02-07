using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TreeEditor.TreeEditorHelper;

public class DebugRoom : MonoBehaviour
{
    [SerializeField] private DebugEdge _edgePrefab;

    private DebugRoom _parentRoom;
    private List<DebugRoom> _childRooms = new List<DebugRoom>();
    private RoomType _roomType = RoomType.None;
    private List<DebugEdge> _edges = new List<DebugEdge>();

    public void BuildRoom(DebugRoom parent, RoomType type = RoomType.None)
    {
        _parentRoom = parent;
        SetType(type);
    }

    public RoomType GetRandomType()
    {
        int maxLength = (int)RoomType.Length;
        int randomIndex = Random.Range(3, maxLength);
        return (RoomType)randomIndex;
    }
    private void SetType(RoomType type)
    {
        _roomType = type;
        Material mat = GetComponent<MeshRenderer>().material;

        switch (_roomType)
        {
            case RoomType.None:
                mat.color = Color.black;
                break;
            case RoomType.Spawn:
                mat.color = Color.white;
                break;
            case RoomType.Exploration:
                mat.color = Color.blue;
                break;
            case RoomType.Arena:
                mat.color = Color.yellow;
                break;
            case RoomType.Boss:
                mat.color = Color.red;
                break;
            case RoomType.Length:
                break;
            default:
                break;
        }
    }

    public void AddChild(DebugRoom childRoom)
    {
        _childRooms.Add(childRoom);
        DebugEdge newEdge = Instantiate<DebugEdge>(_edgePrefab);
        newEdge.BuildLine(this, childRoom);
        _edges.Add(newEdge);
    }

    public void UpdateEdges()
    {
        foreach (DebugEdge edge in _edges)
        {
            edge.BuildLine(this, edge.ChildNode);
        }
    }

}
