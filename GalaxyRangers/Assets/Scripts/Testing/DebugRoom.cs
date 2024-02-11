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
    public Color currentColor;
    public Color originalColor;
    private Material _mat;

    private void Update()
    {
        if (room.specialEventType > 0 && _mat != null)
        {
            float pingPong = Mathf.PingPong(Time.time / 2, 1f);
            if (room.specialEventType == 1)
            {
                //Second chance
                _mat.color = Color.Lerp(currentColor, new Color(0f, 1f, 0f, 1f), pingPong);
            }
            else if (room.specialEventType == 2)
            {
                //Gas
                _mat.color = Color.Lerp(currentColor, new Color(1f, 0f, 0f, 1f), pingPong);
            }
            else return;
        }
    }

    private void OnDestroy()
    {
        foreach(DebugEdge edge in _edges)
        {
            if (edge != null)
            {
                Destroy(edge.gameObject);
            }
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
        _mat = GetComponent<MeshRenderer>().material;
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
                _mat.color = Color.black;
                originalColor = Color.black;
                break;
            case RoomType.Spawn:
                _mat.color = Color.white;
                originalColor = Color.white;
                break;
            case RoomType.Boss:
                _mat.color = Color.red;
                originalColor = Color.red;
                break;
            case RoomType.Exploration:
                _mat.color = Color.blue * colorMult;
                originalColor = Color.blue * colorMult;
                break;
            case RoomType.Arena:
                _mat.color = Color.yellow * colorMult;
                originalColor = Color.yellow * colorMult;
                break;
            case RoomType.Shop:
                _mat.color = Color.cyan;
                originalColor = Color.cyan;
                break;
            case RoomType.Heal:
                _mat.color = Color.green;
                originalColor = Color.green;
                break;
            case RoomType.Item:
                _mat.color = Color.magenta;
                originalColor = Color.magenta;
                break;
            case RoomType.Length:
                break;
            default:
                break;
        }
        currentColor = originalColor;
    }

    public void EnterRoom()
    {
        _mat.color = Color.cyan;
        currentColor = Color.cyan;
    }

    public void ExitRoom()
    {
        _mat.color = originalColor;
        currentColor = originalColor;
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
