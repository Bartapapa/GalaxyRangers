using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BCC_Boss : BaseCharacterController
{
    [Header("WAYPOINTS")]
    [Space]
    public List<Transform> wayPoints = new List<Transform>();
    public Transform centerWayPoint;
    [SerializeField][ReadOnlyInspector] private Transform _currentWayPoint;
    private List<Transform> _unvisitedWayPoints = new List<Transform>();

    public bool isAtCurrentWayPoint { get { return _currentWayPoint ? Vector3.Distance(characterCenter, _currentWayPoint.position) <= 3f : false; } }

    protected override void Start()
    {
        base.Start();

        hasHyperArmor = true;
        _isJumping = true;

        ReinitializeUnvisitedWaypoints();
        _currentWayPoint = GetRandomWayPoint();
    }

    private void ReinitializeUnvisitedWaypoints()
    {
        foreach (Transform wayPoint in wayPoints)
        {
            _unvisitedWayPoints.Add(wayPoint);
        }
    }

    protected override void FixedUpdate()
    {
        FixedCacheData();

        Gravity();

        GroundDetection();
        WallDetection();
        CeilingDetection();

        CharacterOrientation();
    }

    public void OnWayPointAttained()
    {
        _unvisitedWayPoints.Remove(_currentWayPoint);
        if (_unvisitedWayPoints.Count == 0)
        {
            ReinitializeUnvisitedWaypoints();
        }
        _currentWayPoint = GetRandomUnvisitedWayPoint();
    }

    public void MoveToCurrentWayPoint()
    {
        if (_currentWayPoint != null)
        {
            //Movement logic
            Vector2 direction = (Vector2)_currentWayPoint.position - (Vector2)characterCenter;
            direction = direction.normalized;

            //Lerp from actual velocity to desired velocity
            Vector2 lerpdVel = Vector2.Lerp(rigidbodyVelocity, direction * maxSpeed, acceleration * Time.deltaTime);
            SetRigidbodyVelocity(lerpdVel);
        }
        else
        {
            Vector2 lerpdVel = Vector2.Lerp(rigidbodyVelocity, Vector2.zero, acceleration * Time.deltaTime);
            SetRigidbodyVelocity(lerpdVel);
        }
    }

    public Transform GetRandomWayPoint()
    {
        int randomInt = UnityEngine.Random.Range(0, wayPoints.Count);
        return wayPoints[randomInt];
    }

    public Transform GetRandomUnvisitedWayPoint()
    {
        int randomInt = UnityEngine.Random.Range(0, _unvisitedWayPoints.Count);
        return _unvisitedWayPoints[randomInt];
    }
}
