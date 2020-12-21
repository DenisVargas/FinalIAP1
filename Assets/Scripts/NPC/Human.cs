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

        var pursue = GetComponent<PursueState>();
        pursue.getCurrentTarget = getTarget;
        pursue.SwitchState = _states.Feed;
        pursue.AttachTo(_states);

        var attack = GetComponent<AttackState>();
        attack.swithStateTo = _states.Feed;
        attack.getCurrentTarget = getTarget;
        attack.AttachTo(_states);

        var dead = GetComponent<DeadState>();
        dead.AttachTo(_states);

        idle.AddTransition(dead)
            .AddTransition(pursue);

        pursue.AddTransition(attack)
              .AddTransition(idle);

        attack.AddTransition(idle)
              .AddTransition(pursue)
              .AddTransition(dead);

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
            healthDisplay.FadeOut();
            OnDie(this);
            _states.Feed(CommonState.dead);
        }

        if (currentTarget == null)
        {
            var newTarget = inputDamage.source.GetComponent<IDamageable<Damage, HitResult>>();
            if (newTarget != null)
            {
                currentTarget = newTarget;
                _states.Feed(CommonState.attack);
            }
        }

        return result;
    }

    public override void onHit(HitResult hitResult)
    {
        if (hitResult.killed)
        {
            //Busco un nuevo target, porque el actual ha morido!
            currentTarget = null;
            _states.Feed(CommonState.idle);
        }
    }
}
