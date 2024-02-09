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
        SetColor(room.roomType, room.scenario);
    }

    public RoomType GetRandomType()
    {
        int maxLength = (int)RoomType.Length;
        int randomIndex = Random.Range(3, maxLength);
        return (RoomType)randomIndex;
    }
    private void SetColor(RoomType type, DifficultyScenario scenario)
    {
        _roomType = type;
        Material mat = GetComponent<MeshRenderer>().material;
        float colorMult = 1;
        switch (scenario)
        {
            case DifficultyScenario.None:
                colorMult = 0;
                break;
            case DifficultyScenario.Easy:
                colorMult = 1;
                break;
            case DifficultyScenario.Medium:
                colorMult = .7f;
                break;
            case DifficultyScenario.Hard:
                colorMult = .45f;
                break;
        }

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
            case RoomType.Boss:
                mat.color = Color.red;
                originalColor = Color.red;
                break;
            case RoomType.Exploration:
                mat.color = Color.blue * colorMult;
                originalColor = Color.blue * colorMult;
                break;
            case RoomType.Arena:
                mat.color = Color.yellow * colorMult;
                originalColor = Color.yellow * colorMult;
                break;
            case RoomType.Shop:
                mat.color = Color.cyan;
                originalColor = Color.cyan;
                break;
            case RoomType.Heal:
                mat.color = Color.green;
                originalColor = Color.green;
                break;
            case RoomType.Item:
                mat.color = Color.magenta;
                originalColor = Color.magenta;
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

    public void AddTeleportDestination(Transform teleportDestination)
    {
        DebugEdge newEdge = Instantiate<DebugEdge>(_edgePrefab);
        newEdge.BuildTeleport(this.transform, teleportDestination);
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
