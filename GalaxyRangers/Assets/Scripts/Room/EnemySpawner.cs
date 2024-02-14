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
    private CharacterHealth _chosenEnemy;
    private bool _enemyDied = false;
    public bool enemyDied { get { return _enemyDied; } }

    public void GenerateEnemy()
    {
        if (_chosenEnemy != null)
        {
            RegenerateEnemy();
        }
        else
        {
            CharacterHealth chosenEnemy = GetEnemy();
            if (chosenEnemy == null)
                return;

            CharacterHealth newEnemy = Instantiate<CharacterHealth>(chosenEnemy, this.transform);
            _chosenEnemy = chosenEnemy; //Ref to prefab, not instantiated enemy
            newEnemy.transform.parent = WorldManager.Instance.currentRogueRoom.resetParent;
        }
    }

    private void RegenerateEnemy()
    {
        if (_enemyDied)
            return;

        CharacterHealth newEnemy = Instantiate<CharacterHealth>(_chosenEnemy, this.transform);
        newEnemy.transform.parent = WorldManager.Instance.currentRogueRoom.resetParent;
    }

    private CharacterHealth GetEnemy()
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
}
