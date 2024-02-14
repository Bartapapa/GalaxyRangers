using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GalaxyRangers/LD/EnemyFolder")]
public class EnemyFolder : ScriptableObject
{
    public AIBrain_Base meleeNormalPrefab;
    public AIBrain_Base meleeElitePrefab;
    [Space]
    public AIBrain_Base rangedNormalPrefab;
    public AIBrain_Base rangedElitePrefab;
    [Space]
    public AIBrain_Base bossPrefab;
}
