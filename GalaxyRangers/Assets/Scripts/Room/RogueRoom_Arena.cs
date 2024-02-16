using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RogueRoom_Arena : RogueRoom
{
    //Cached
    protected bool _hasEnded = false;
    public bool hasEnded { get { return _hasEnded; } }
    protected bool _hasStarted = false;
    public bool hasStarted { get { return _hasStarted; } }

    //Coroutines
    protected Coroutine arenaStartCoroutine;

    public override void BuildRoom(Room room)
    {
        if (room == null)
            return;

        roomData = room;

        //Open doors entry and exit points, close the rest.
        BuildTraversalPoints(room);

        //Set scenario.
        SetScenario(room);

        //Start arena.
        if (_hasEnded)
            return;

        arenaStartCoroutine = StartCoroutine(CoArenaStart());

        _teleporter.canBeActivated = false;
    }

    private IEnumerator CoArenaStart()
    {
        yield return new WaitForSeconds(1f);
        StartArena();
        arenaStartCoroutine = null;
    }

    protected virtual void StartArena()
    {
        Debug.Log("Closed all exits.");
        //Close exits.
        List<TraversalLocation> allTraversalLocations = GetAllTraversalLocations();
        foreach(TraversalLocation traversal in allTraversalLocations)
        {
            CloseExitPoint(traversal);
        }

        //Set camera to arena mode.
        CameraManager.Instance.CameraState = CameraState.Arena;

        _hasStarted = true;
    }

    protected virtual void EndArena()
    {
        //Open exits.
        BuildTraversalPoints(roomData);

        //Set camera to exploration mode.
        CameraManager.Instance.CameraState = CameraState.Exploration;

        //Force activate teleporter.
        ToggleTeleporterActivation(true, true);

        _hasEnded = true;
    }
}
