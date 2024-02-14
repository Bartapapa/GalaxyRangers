using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType
{
    None,
    Melee,
    Ranged,
    Random,
}

public enum EnemyRank
{
    None,
    Normal,
    Elite,
    Random,
}
public class EnemySpawner : MonoBehaviour
{
    [Header("FORCE ENEMY TYPE")]
    [SerializeField] private EnemyType _enemyType = EnemyType.Random;

    [Header("FORCE ENEMY RANK")]
    [SerializeField] private EnemyRank _enemyRank = EnemyRank.Random;

    //Saved info cache
    private AIBrain_Base _chosenEnemy;
    private AIBrain_Base _instantiatedEnemy;
    private bool _enemyDied = false;
    public bool enemyDied { get { return _enemyDied; } }

    private void OnDisable()
    {
        if (_instantiatedEnemy)
        {
            _instantiatedEnemy.health.CharacterDied -= OnEnemyDied;
        }
    }

    public AIBrain_Base GenerateEnemy()
    {
        if (_chosenEnemy != null)
        {
            return RegenerateEnemy();
        }
        else
        {
            AIBrain_Base chosenEnemy = GetEnemy();
            if (chosenEnemy == null)
                return null;

            AIBrain_Base newEnemy = Instantiate<AIBrain_Base>(chosenEnemy, this.transform);
            _chosenEnemy = chosenEnemy; //Ref to prefab, not instantiated enemy
            _instantiatedEnemy = newEnemy;

            Debug.Log(WorldManager.Instance);
            Debug.Log(WorldManager.Instance.currentRogueRoom);
            Debug.Log(WorldManager.Instance.currentRogueRoom.resetParent);
            newEnemy.transform.parent = WorldManager.Instance.currentRogueRoom.resetParent;
            newEnemy.health.CharacterDied -= OnEnemyDied;
            newEnemy.health.CharacterDied += OnEnemyDied;
            return newEnemy;
        }
    }

    private AIBrain_Base RegenerateEnemy()
    {
        if (_enemyDied)
            return null;

        AIBrain_Base newEnemy = Instantiate<AIBrain_Base>(_chosenEnemy, this.transform);
        newEnemy.transform.parent = WorldManager.Instance.currentRogueRoom.resetParent;
        _instantiatedEnemy = newEnemy;

        newEnemy.health.CharacterDied -= OnEnemyDied;
        newEnemy.health.CharacterDied += OnEnemyDied;
        return newEnemy;
    }

    private AIBrain_Base GetEnemy()
    {
        switch (_enemyType)
        {
            case EnemyType.None:
                return null;
            case EnemyType.Melee:
                switch (_enemyRank)
                {
                    case EnemyRank.None:
                        return null;
                    case EnemyRank.Normal:
                        return WorldManager.Instance.enemyFolder.meleeNormalPrefab;
                    case EnemyRank.Elite:
                        return WorldManager.Instance.enemyFolder.meleeElitePrefab;
                    case EnemyRank.Random:
                        int randomInt = UnityEngine.Random.Range(0, 2);
                        return randomInt == 0 ? WorldManager.Instance.enemyFolder.meleeNormalPrefab : WorldManager.Instance.enemyFolder.meleeElitePrefab;
                    default:
                        return null;
                }
            case EnemyType.Ranged:
                switch (_enemyRank)
                {
                    case EnemyRank.None:
                        return null;
                    case EnemyRank.Normal:
                        return WorldManager.Instance.enemyFolder.rangedNormalPrefab;
                    case EnemyRank.Elite:
                        return WorldManager.Instance.enemyFolder.rangedElitePrefab;
                    case EnemyRank.Random:
                        int randomInt = UnityEngine.Random.Range(0, 2);
                        return randomInt == 0 ? WorldManager.Instance.enemyFolder.rangedNormalPrefab : WorldManager.Instance.enemyFolder.rangedElitePrefab;
                    default:
                        return null;
                }
            case EnemyType.Random:
                switch (_enemyRank)
                {
                    case EnemyRank.None:
                        return null;
                    case EnemyRank.Normal:
                        int randomInt0 = UnityEngine.Random.Range(0, 2);
                        return randomInt0 == 0 ? WorldManager.Instance.enemyFolder.meleeNormalPrefab : WorldManager.Instance.enemyFolder.rangedNormalPrefab;
                    case EnemyRank.Elite:
                        int randomInt1 = UnityEngine.Random.Range(0, 2);
                        return randomInt1 == 0 ? WorldManager.Instance.enemyFolder.meleeElitePrefab : WorldManager.Instance.enemyFolder.rangedElitePrefab;
                    case EnemyRank.Random:
                        int randomInt2 = UnityEngine.Random.Range(0, 4);
                        if (randomInt2 == 0)
                        {
                            return WorldManager.Instance.enemyFolder.meleeNormalPrefab;
                        }
                        else if (randomInt2 == 1)
                        {
                            return WorldManager.Instance.enemyFolder.meleeElitePrefab;
                        }
                        else if (randomInt2 == 2)
                        {
                            return WorldManager.Instance.enemyFolder.rangedNormalPrefab;
                        }
                        else if (randomInt2 == 3)
                        {
                            return WorldManager.Instance.enemyFolder.rangedElitePrefab;
                        }
                        else
                        {
                            return null;
                        }
                    default:
                        return null;
                }

        }
        return null;
    }

    private void OnEnemyDied(CharacterHealth health)
    {
        _enemyDied = true;
        health.CharacterDied -= OnEnemyDied;
    }

    public void ResetSpawner()
    {
        _chosenEnemy = null;
        _enemyDied = false;
    }
}
