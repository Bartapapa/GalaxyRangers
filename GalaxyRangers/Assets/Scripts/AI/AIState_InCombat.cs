using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class AIState_InCombat : AIState
{
    [SerializeField] private AIState chaseState;
    [SerializeField] private AIState fleeState;

    private List<WeaponAttack> lightCombo = new List<WeaponAttack>();
    private List<WeaponAttack> heavyComboBreak = new List<WeaponAttack>();
    private WeaponAttack heavyAttack;
    private float _minAttackDistance = float.MaxValue;
    private float _maxAttackDistance = float.MinValue;

    private bool _attacksInitialized = false;
    private Weapon currentWeapon;

    public override AIreturn Tick(AIBrain_Base brain)
    {
        PlayerInputs simulatedInputs = new PlayerInputs();
        //Initialize list of attacks and current weapon if not done.
        if (currentWeapon != brain.combat.currentWeapon)
        {
            lightCombo.Clear();
            _attacksInitialized = false;
            _minAttackDistance = float.MaxValue;
            _maxAttackDistance = float.MinValue;
        }

        if (!_attacksInitialized) InitializeAttacks(brain);

        if (brain.currentPlayerDistance > brain.combatDistance)
        {
            //Return to chase.
            Debug.Log("Player outside combatdistance.");
            return new AIreturn(chaseState, simulatedInputs);
        }

        //if (brain.currentPlayerDistance < _minAttackDistance)
        //{
        //    //Go to fleeing. Make sure to force CharacterOrientation to be in the player's direction.
        //    return new AIreturn(fleeState, simulatedInputs);
        //}

        if (brain.currentPlayerDistance > _maxAttackDistance)
        {
            //Return to chase.
            Debug.Log("Player outside maxAttackDistance.");
            return new AIreturn(chaseState, simulatedInputs);
        }

        if (brain.canPerformNewAttack)
        {
            //Get all possible attacks taking into account the player's current distance.
            //Choose one random attack among all possibilities.
            //Attack.

            int attackType = ChooseRandomAttack(brain);
            if (attackType == 0)
            {
                //No attack found.
                //Return to chase.
                Debug.Log("No attack found.");
                return new AIreturn(chaseState, simulatedInputs);
            }
            else if (attackType == 1)
            {
                //Light attack.
                brain.combat.RequestLightAttack();
                brain.attackCooldown = brain.combat.currentWeaponStrike.attack.comboEndAttackCooldown;
            }
            else if (attackType == 2)
            {
                //Heavy attack.
                brain.combat.RequestHeavyAttack();
                brain.attackCooldown = brain.combat.currentWeaponStrike.attack.comboEndAttackCooldown;
            }
            else
            {
                //Default.
                //Return to chase.
                Debug.Log("Defaulted.");
                return new AIreturn(chaseState, simulatedInputs);
            }
        }

        //Default
        return new AIreturn(this, simulatedInputs);
    }

    private void InitializeAttacks(AIBrain_Base brain)
    {
        currentWeapon = brain.combat.currentWeapon;

        List<WeaponAttack> allAttacks = new List<WeaponAttack>();

        foreach (WeaponStrike lightstrike in brain.combat.currentWeapon.lightCombo)
        {
            lightCombo.Add(lightstrike.attack);
            allAttacks.Add(lightstrike.attack);
        }
        foreach (WeaponStrike heavystrike in brain.combat.currentWeapon.heavyComboBreaks)
        {
            heavyComboBreak.Add(heavystrike.attack);
            allAttacks.Add(heavystrike.attack);
        }

        heavyAttack = brain.combat.currentWeapon.heavyAttack.attack;

        allAttacks.Add(brain.combat.currentWeapon.heavyAttack.attack);

        foreach(WeaponAttack attack in allAttacks)
        {
            if(attack.minAttackDistance < _minAttackDistance)
            {
                _minAttackDistance = attack.minAttackDistance;
            }
            if (attack.maxAttackDistance > _maxAttackDistance)
            {
                _maxAttackDistance = attack.maxAttackDistance;
            }
        }

        _attacksInitialized = true;
        Debug.Log("Attacks initialized!");
    }

    private int ChooseRandomAttack(AIBrain_Base brain)
    {
        //This only takes light attacks and heavy attacks - no combos, or combo breaks.      
        List<WeaponAttack> allAttacks = new List<WeaponAttack>();
        allAttacks.Add(lightCombo[0]);
        allAttacks.Add(heavyAttack);

        //Resolve attacks by potential in terms of distance.
        List<WeaponAttack> allAttacksInDistance = new List<WeaponAttack>();
        foreach (WeaponAttack attack in allAttacks)
        {
            if (attack.minAttackDistance <= brain.currentPlayerDistance && attack.maxAttackDistance >= brain.currentPlayerDistance)
            {
                allAttacksInDistance.Add(attack);
            }
        }

        if (allAttacksInDistance.Count <= 0)
        {
            Debug.Log("No attacks in range.");
            return 0;
        }
        if (allAttacksInDistance.Count == 1) return allAttacksInDistance[0].attackType;

        //Resolve attacks by priority.
        int highestPriority = 0;
        List<WeaponAttack> allHighestPriorityAttacks = new List<WeaponAttack>();
        foreach (WeaponAttack attack in allAttacksInDistance)
        {
            if (attack.attackPriority > highestPriority)
            {
                highestPriority = attack.attackPriority;
            }
        }
        foreach(WeaponAttack attack in allAttacksInDistance)
        {
            if (attack.attackPriority >= highestPriority)
            {
                allHighestPriorityAttacks.Add(attack);
            }
        }

        if (allHighestPriorityAttacks.Count <= 0)
        {
            Debug.Log("No attacks have priority.");
            return 0;
        }
        if (allHighestPriorityAttacks.Count == 1) return allHighestPriorityAttacks[0].attackType;

        //Resolve attacks by weight.
        int maxWeight = 1;
        foreach(WeaponAttack attack in allHighestPriorityAttacks)
        {
            maxWeight += attack.attackWeight;
        }
        int randomInt = UnityEngine.Random.Range(1, maxWeight);
        int previousWeight = 1;
        for (int k = 0; k < allHighestPriorityAttacks.Count; k++)
        {
            if (randomInt < allHighestPriorityAttacks[k].attackWeight + previousWeight)
            {
                return allHighestPriorityAttacks[k].attackType;
            }
            else
            {
                previousWeight += allHighestPriorityAttacks[k].attackWeight;
            }
        }

        Debug.Log("Nothing found.");
        return 0;
    }
}
