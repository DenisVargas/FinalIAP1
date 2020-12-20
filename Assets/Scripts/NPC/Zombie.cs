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
    [SerializeField] bool LiderUnit = false;
    [Tooltip("Permite que la unidad se maneje de manera autónoma.")]
    [SerializeField] bool _disband = false;

    [SerializeField] CommonState Debug_CurrentState;

    private void Awake()
    {
        if (!anims)
            anims = GetComponent<Animator>();
        if (!solver)
            solver = GetComponent<PathFindSolver>();

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
        move.SetAnimator(anims)
          .AttachTo(_states);

        var dead = GetComponent<DeadState>().SetAnimator(anims).AttachTo(_states);

        #region Transitions
        idle.AddTransition(move, (cs) => { print("Transitioning!"); })
            .AddTransition(followLeader)
            .AddTransition(attack)
            .AddTransition(pursue)
            .AddTransition(dead, (cs) => { print("Transitioning to Dead from Idle"); });

        pursue.AddTransition(attack)
              .AddTransition(dead)
              .AddTransition(idle);

        attack.AddTransition(idle);

        move.AddTransition(idle, (cs) => { print("Transitioning!"); })
          .AddTransition(attack)
          .AddTransition(followLeader)
          .AddTransition(dead, (cs) => { });

        dead.AddTransition(dead, (cs) => { print("Transitioning"); })
            .AddTransition(idle, (cs) => { });
        #endregion

        if (_disband)
            _states.SetState(CommonState.idle);
        //if (_states.getCurrentStateType() != CommonState.followLeader && !LiderUnit)
        //    _states.Feed(CommonState.followLeader);
    }

    private void Update()
    {
        if (LiderUnit)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _states.Feed(CommonState.moveTo);
            }
        }

        _states.Update();
        Debug_CurrentState = _states.getCurrentStateType();
    }
}
