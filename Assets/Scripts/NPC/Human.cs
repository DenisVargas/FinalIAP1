using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IA.FSM;
using System;

public class Human : NPC
{
    IDamageable<Damage, HitResult> currentTarget = null;

    public CommonState debug_currentState;

    private void Awake()
    {
        if (healthDisplay)
            healthDisplay.value = 1f;
        _states = new FiniteStateMachine<CommonState>();

        Action lookForZombies = () =>
        {
            currentTarget = FindCloserTarget("Zombie", sight.range, sight.visibles);
            if (currentTarget != null)
            {
                //Encontramos un objetivo
                _states.Feed(CommonState.pursue);
            }
        };
        Func<IDamageable<Damage, HitResult>> getTarget = () =>
        {
            return currentTarget;
        };

        var idle = GetComponent<IdleState>();
        idle.CheckLineOfSight = lookForZombies;
        idle.AttachTo(_states, true);

        var attack = GetComponent<AttackState>();
        attack.getCurrentTarget = getTarget;
        attack.AttachTo(_states);

        var dead = GetComponent<DeadState>();
        dead.AttachTo(_states);

        idle.AddTransition(dead);
        dead.AddTransition(idle);
    }

    private void Update()
    {
        _states.Update();
        debug_currentState = _states.getCurrentStateType();
    }

    public override HitResult getHit(Damage inputDamage)
    {
        HitResult result = new HitResult();

        Debug.Log($"{gameObject.name} ha recibido daño.");

        health = (health - inputDamage.damageAmmount);
        if (health < 0)
            health = 0;

        if (healthDisplay)
            healthDisplay.value = ((float)health) / ((float)maxhealth);

        if (health <= 0)
        {
            Debug.Log($"{gameObject.name} ha morido.");
            result.killed = true;

            OnDie(this);
            _states.Feed(CommonState.dead);
        }

        return result;
    }

    public override void onHit(HitResult hitResult)
    {
        if (hitResult.killed)
        {
            //Busco un nuevo target, porque el actual ha morido!
        }
    }
}
