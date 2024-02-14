using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RogueRoom_Arena_Enemy : RogueRoom_Arena
{
    [Header("ENEMY WAVES")]
    [SerializeField] private int _numberOfWaves;

    [Header("SOFTLOCK PREVENTION")]
    [SerializeField] private float _forceNewWaveAfterTime = 60f;

    //Cached
    private int _currentWaveNumber = 0;
    private float _currentWaveTimer = 0f;
    private int _currentNumberOfEnemies = 0;

    private bool allEnemiesDied { get { return _currentNumberOfEnemies <= 0; } }

    private List<AIBrain_Base> _instantiatedEnemies = new List<AIBrain_Base>();

    private void Update()
    {
        if (!_hasEnded)
        {
            HandleCurrentWaveTimer();
        }
    }

    private void HandleCurrentWaveTimer()
    {
        if (_currentWaveTimer > 0)
        {
            _currentWaveTimer -= Time.deltaTime;
        }
        else
        {
            StartNewWave();
        }
    }

    protected override void StartArena()
    {
        base.StartArena();
        //Spawn waves, set newly made enemies into arena camera focus.
        StartNewWave();
    }

    private void StartNewWave()
    {
        if (_currentWaveNumber >= _numberOfWaves)
        {
            //Has attained max number of waves
            EndArena();
        }
        else
        {
            _currentWaveNumber++;
            //Get each enemy spawner, spawn an enemy. Add enemy as focustarget in Camera. Make enemy AI focus player.
            foreach(EnemySpawner spawner in _chosenScenario.enemySpawners)
            {

                AIBrain_Base newEnemy = spawner.GenerateEnemy();
                if (newEnemy == null)
                    break;

                newEnemy.health.CharacterDied -= OnEnemyDeath;
                newEnemy.health.CharacterDied += OnEnemyDeath;
                CameraManager.Instance.AddFocusObjectToCamera(newEnemy.transform);

                _instantiatedEnemies.Add(newEnemy);
                _currentNumberOfEnemies++;

                spawner.ResetSpawner();
            }

        }
    }

    private void OnEnemyDeath(CharacterHealth health)
    {
        _currentNumberOfEnemies--;

        if (allEnemiesDied)
        {
            StartNewWave();
            _currentWaveTimer = _forceNewWaveAfterTime;
        }

        foreach(AIBrain_Base ai in _instantiatedEnemies)
        {
            if (health == ai.health)
            {
                _instantiatedEnemies.Remove(ai);
                break;
            }
        }

        health.CharacterDied -= OnEnemyDeath;
    }

    public override void ResetRoom()
    {
        _instantiatedEnemies.Clear();
        base.ResetRoom();
    }

}
