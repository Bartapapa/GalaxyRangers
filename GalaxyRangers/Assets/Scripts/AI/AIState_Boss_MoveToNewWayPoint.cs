using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class AIState_Boss_MoveToNewWayPoint : AIState
{
    [SerializeField] private AIState idleState;
    [SerializeField] private AIState moveToCenter;

    [Header("PARAMETERS")]
    [Space]
    public int minNumberOfVisitedWayPoints = 3;
    public int maxNumberOfVisitedWayPoints = 8;

    [Header("WAYPOINTS")]
    [Space]
    public List<Transform> wayPoints = new List<Transform>();
    public Transform centerWayPoint;
    [SerializeField][ReadOnlyInspector] private Transform _currentWayPoint;
    private List<Transform> _unvisitedWayPoints = new List<Transform>();

    private AIBrain_Base _currentBrain;
    private int _visitedWaypointsIndex = 0;
    private int _currentVisitedWayPointsIndexDestination = 0;
    private bool _initialized = false;

    public bool isAtCurrentWayPoint { get { return _currentWayPoint ? Vector3.Distance(_currentBrain.controller.characterCenter, _currentWayPoint.position) <= 3f : false; } }

    public override AIreturn Tick(AIBrain_Base brain)
    {
        PlayerInputs simulatedInputs = new PlayerInputs();
        if (!_initialized)
        {
            InitializeState(brain);
        }

        if (_currentWayPoint == null && !isAtCurrentWayPoint)
        {
            _currentWayPoint = GetRandomWayPoint();
        }

        MoveToCurrentWayPoint(brain);

        if (isAtCurrentWayPoint)
        {
            _visitedWaypointsIndex++;
            if (_visitedWaypointsIndex >= _currentVisitedWayPointsIndexDestination)
            {
                ResetState();
                return new AIreturn(moveToCenter, simulatedInputs);
            }
            OnWayPointAttained();
            return new AIreturn(this, simulatedInputs);
        }

        //Default
        return new AIreturn(this, simulatedInputs);
    }

    public override void ResetState()
    {
        _initialized = false;

        _visitedWaypointsIndex = 0;
        _currentWayPoint = null;
    }

    private void InitializeState(AIBrain_Base brain)
    {
        _currentBrain = brain;

        _currentVisitedWayPointsIndexDestination = UnityEngine.Random.Range(minNumberOfVisitedWayPoints, maxNumberOfVisitedWayPoints + 1);

        _initialized = true;
    }

    private void OnWayPointAttained()
    {
        _unvisitedWayPoints.Remove(_currentWayPoint);
        if (_unvisitedWayPoints.Count == 0)
        {
            ReinitializeUnvisitedWaypoints();
        }
        _currentWayPoint = GetRandomUnvisitedWayPoint();
    }

    private void MoveToCurrentWayPoint(AIBrain_Base brain)
    {
        if (_currentWayPoint != null)
        {
            //Movement logic
            Vector2 direction = (Vector2)_currentWayPoint.position - (Vector2)brain.controller.characterCenter;
            direction = direction.normalized;

            //Lerp from actual velocity to desired velocity
            Vector2 lerpdVel = Vector2.Lerp(brain.controller.rigidbodyVelocity, direction * brain.controller.maxSpeed, brain.controller.acceleration * Time.deltaTime);
            brain.controller.SetRigidbodyVelocity(lerpdVel);
        }
    }

    private Transform GetRandomWayPoint()
    {
        int randomInt = UnityEngine.Random.Range(0, wayPoints.Count);
        return wayPoints[randomInt];
    }

    private Transform GetRandomUnvisitedWayPoint()
    {
        int randomInt = UnityEngine.Random.Range(0, _unvisitedWayPoints.Count);
        return _unvisitedWayPoints[randomInt];
    }

    private void ReinitializeUnvisitedWaypoints()
    {
        foreach (Transform wayPoint in wayPoints)
        {
            _unvisitedWayPoints.Add(wayPoint);
        }
    }
}
