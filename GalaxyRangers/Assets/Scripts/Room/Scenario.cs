using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scenario : MonoBehaviour
{
    [Header("ENEMY SPAWNERS")]
    [SerializeField] private List<EnemySpawner> _enemySpawners = new List<EnemySpawner>();
    public List<EnemySpawner> enemySpawners { get { return _enemySpawners; } }
}
