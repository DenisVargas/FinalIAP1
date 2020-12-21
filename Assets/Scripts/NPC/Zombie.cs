using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IA.FSM;
using IA.PathFinding;

public class Zombie : NPC
{
    [SerializeField] IDamageable<Damage, HitResult> currentTarget = null;
    [SerializeField] Node _initialTarget;
    [Tooltip("Marca a esta unidad como lider de un grupo")]
    public bool isCaptain = false;
    [Tooltip("Permite que la unidad se maneje de manera autónoma.")]
    [SerializeField] bool _isIndependant = false;

    [SerializeField] CommonState Debug_CurrentState;

    private void Awake()
    {
        if (!anims)
            anims = GetComponent<Animator>();
        if (!solver)
            solver = GetComponent<PathFindSolver>();

        health = maxhealth;

        //State Machine
        _states = new FiniteStateMachine<CommonState>();

        Action lookForHumans = () =>
        {
            currentTarget = FindCloserTarget("Human", sight.range, sight.visibles);
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
        Action ReferenceTargetReached = () =>
        {
            //Si somos líderes...
            //Cuando nos movemos a un punto de referencia y llegamos a dicho punto
            //Entonces...

            _states.Feed(CommonState.idle);
        };

        var idle = GetComponent<IdleState>();
        idle.CheckLineOfSight = lookForHumans;
        idle.AttachTo(_states, true);

        //Follow Leader
        FollowLeaderState followLeader = GetComponent<FollowLeaderState>();
        followLeader.LookForTargets = lookForHumans;
        followLeader.AttachTo(_states);

        //pursue
        var pursue = GetComponent<PursueState>();
        pursue.getCurrentTarget = getTarget;
        pursue.SwitchState = _states.Feed;
        pursue.AttachTo(_states);

        //Attack
        AttackState attack = GetComponent<AttackState>();
        attack.swithStateTo = _states.Feed;
        attack.getCurrentTarget = getTarget;
        attack.AttachTo(_states);

        //Move
        MoveToState move = GetComponent<MoveToState>();
        move.lookForTargets = lookForHumans;
        move.OnReachedTarget = ReferenceTargetReached;
        move.getEndTargetPoint = () => { return _initialTarget; };
        move.AttachTo(_states);

        var dead = GetComponent<DeadState>()
                   .SetAnimator(anims)
                   .AttachTo(_states);

        #region Transitions
        idle.AddTransition(move, (cs) => { print("Transitioning!"); })
            .AddTransition(followLeader)
            .AddTransition(attack)
            .AddTransition(pursue)
            .AddTransition(dead, (cs) => { print("Transitioning to Dead from Idle"); });

        followLeader.AddTransition(move)
                    .AddTransition(pursue);

        pursue.AddTransition(attack)
              .AddTransition(dead)
              .AddTransition(idle);

        attack.AddTransition(idle)
              .AddTransition(dead);

        move.AddTransition(idle, (cs) => { print("Transitioning!"); })
            .AddTransition(attack)
            .AddTransition(pursue)
            .AddTransition(followLeader)
            .AddTransition(dead, (cs) => { });

        dead.AddTransition(dead, (cs) => { print("Transitioning"); })
            .AddTransition(idle, (cs) => { });
        #endregion

        if (isCaptain)
            _states.Feed(CommonState.moveTo);
        else if (!isCaptain && !_isIndependant)
            _states.Feed(CommonState.followLeader);
    }

    private void Update()
    {
        if (isCaptain)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _states.Feed(CommonState.moveTo);
            }
        }

        _states.Update();
        Debug_CurrentState = _states.getCurrentStateType();
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
        base.onHit(hitResult);
        print("TE cogi puto");
        if (hitResult.killed)
        {
            //Busco un nuevo target, porque el actual ha morido!
            currentTarget = null;
        }
    }

    public void SetLookUpTargetLocation(Vector3 lookUpPosition)
    {
        _initialTarget = solver.getCloserNode(lookUpPosition);
        if (_initialTarget != null && isCaptain)
            _states.Feed(CommonState.moveTo);
    }

    public void SetGroup(List<Transform> Allies)
    {
        var followLeaderStates = GetComponent<FollowLeaderState>();
        if (followLeaderStates != null)
            followLeaderStates.Allies = Allies.ToArray();
    }

    public void SetLeader(Transform leader)
    {
        var FollowLeaderState = GetComponent<FollowLeaderState>();
        if (FollowLeaderState != null)
            FollowLeaderState.SetFollowUpLeader(leader);
    }

    public void FeedState(CommonState state)
    {
        _states.Feed(state);
    }
}
