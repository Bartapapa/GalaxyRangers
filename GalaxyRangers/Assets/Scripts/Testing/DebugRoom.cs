using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TreeEditor.TreeEditorHelper;

public class DebugRoom : MonoBehaviour
{
    [SerializeField] private DebugEdge _edgePrefab;

    public Room room;
    private List<Transform> _childRooms = new List<Transform>();
    private RoomType _roomType = RoomType.None;
    private List<DebugEdge> _edges = new List<DebugEdge>();
    public Color originalColor;

    private void OnDestroy()
    {
        foreach(DebugEdge edge in _edges)
        {
            Destroy(edge.gameObject);
        }
    }
    public void BuildRoom(Room _room)
    {
        room = _room;
        SetType(room.roomType);
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
                originalColor = Color.black;
                break;
            case RoomType.Spawn:
                mat.color = Color.white;
                originalColor = Color.white;
                break;
            case RoomType.Exploration:
                mat.color = Color.blue;
                originalColor = Color.blue;
                break;
            case RoomType.Arena:
                mat.color = Color.yellow;
                originalColor = Color.yellow;
                break;
            case RoomType.Boss:
                mat.color = Color.red;
                originalColor = Color.red;
                break;
            case RoomType.Length:
                break;
            default:
                break;
        }
    }

    public void EnterRoom()
    {
        Material mat = GetComponent<MeshRenderer>().material;

        mat.color = Color.cyan;
    }

    public void ExitRoom()
    {
        Material mat = GetComponent<MeshRenderer>().material;
        mat.color = originalColor;
    }

    public void AddChild(Transform childRoom)
    {
        _childRooms.Add(childRoom);
        DebugEdge newEdge = Instantiate<DebugEdge>(_edgePrefab);
        newEdge.BuildLine(this.transform, childRoom);
        _edges.Add(newEdge);
    }

    public void UpdateEdges()
    {
        foreach (DebugEdge edge in _edges)
        {
            edge.BuildLine(this.transform, edge.ChildNode);
        }
    }

}
