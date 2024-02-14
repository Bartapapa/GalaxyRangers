using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GalaxyRangers/LD/EnemyFolder")]
public class EnemyFolder : ScriptableObject
{
    public CharacterHealth meleeNormalPrefab;
    public CharacterHealth meleeElitePrefab;
    [Space]
    public CharacterHealth rangedNormalPrefab;
    public CharacterHealth rangedElitePrefab;
    [Space]
    public CharacterHealth bossPrefab;
}
