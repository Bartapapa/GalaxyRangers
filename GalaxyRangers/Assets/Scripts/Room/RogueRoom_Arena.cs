using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RogueRoom_Arena : RogueRoom
{
    //Cached
    protected bool _hasEnded = false;
    public bool hasEnded { get { return _hasEnded; } }

    //Coroutines
    protected Coroutine arenaStartCoroutine;

    private void Start()
    {
        if (_hasEnded)
            return;

        arenaStartCoroutine = StartCoroutine(CoArenaStart());
    }

    private IEnumerator CoArenaStart()
    {
        yield return new WaitForSeconds(1f);
        StartArena();
        arenaStartCoroutine = null;
    }

    protected virtual void StartArena()
    {
        //Close exits.
        List<TraversalLocation> allTraversalLocations = GetAllTraversalLocations();
        foreach(TraversalLocation traversal in allTraversalLocations)
        {
            CloseExitPoint(traversal);
        }

        //Set camera to arena mode.
        CameraManager.Instance.CameraState = CameraState.Arena;
    }

    protected virtual void EndArena()
    {
        //Open exits.
        BuildTraversalPoints(roomData);

        //Set camera to exploration mode.
        CameraManager.Instance.CameraState = CameraState.Exploration;

        _hasEnded = true;
    }
}
