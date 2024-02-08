using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("OBJECT REFS & ANIMATION")]
    [Space]
    [SerializeField] private GameObject _WeaponMesh;
    [SerializeField] private int _stanceType = 0;

    [Header("LIGHT COMBO")]
    [Space]
    [SerializeField] private List<WeaponAttack> _lightCombo = new List<WeaponAttack>();
    [SerializeField] private List<HurtBox> _lightComboHurtBoxes = new List<HurtBox>();
    [Space]
    private int _comboCounter = 0;

    [Header("HEAVY COMBO BREAK")]
    [Space]
    [SerializeField] private List<WeaponAttack> _heavyComboBreaks = new List<WeaponAttack>();
    [SerializeField] private List<HurtBox> _heavyComboBreaksHurtBoxes = new List<HurtBox>();
    [Space]

    [Header("HEAVY ATTACK")]
    [Space]
    [SerializeField] private WeaponAttack _heavyAttack;
    [SerializeField] private HurtBox _heavyAttackHurtBox;
    [SerializeField] private float _maxChargedHeavyAttackDamageMultiplier = 2f;
}
