using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GalaxyRangers/Stats/CharacterStats")]
public class CharacterStats : ScriptableObject
{
    [Header("MOVEMENT")]
    [Space]
    public float baseMaxSpeed = 8f;
    public float acceleration = 10f;

    [Header("AIR BEHAVIOR")]
    [Space]
    public float gravity = -40f;
    public float minFallSpeed = -15f;
    public float fastFallGravityFactor = 1.5f;
    public float minFastFallSpeed = -30f;

    [Header("JUMP BEHAVIOR")]
    [Space]
    public int baseMaxJumpCount = 1;
    public int baseMaxWallJumpCount = 1;
    public float baseJumpStrength = 16;
    public float baseMaxJumpDuration = .5f;
    public AnimationCurve jumpDosageCurve = new AnimationCurve();
    [Range(1f, 4f)] public float shortJumpSpeedFactor = 4f;

    [Header("DASH BEHAVIOR")]
    [Space]
    public float baseDashSpeed = 20f;
    public float baseDashDuration = .3f;
    public float baseDashCooldown = .3f;
    public int baseMaxAirDashes = 1;

    [Header("COMBAT")]
    [Space]
    public GameFaction faction = GameFaction.Player;
    public float baseHealth = 5f;
}
