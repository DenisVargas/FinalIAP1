using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IA.FSM;
using IA.PathFinding;

public class PursueState : State
{
    [Header("Stats")]
    [SerializeField] float pursueSpeed = 5f;
    [SerializeField] float attackRange = 2f;

    [Header("PathFinding")]
    [SerializeField] PathFindSolver solver = null;

    Action<CommonState> SwitchState = delegate { };

    IDamageable<Damage, HitResult> Target = null;
    Node nextNode = null;

    public override void Begin()
    {
        _anims.SetBool("Walking", true);
    }

    public override void Execute()
    {
        base.Execute();

        if (Vector3.Distance(Target.transform.position, transform.position) > attackRange)
        {
            var Path = solver.getPathTo(transform.position, Target.transform.position);//Recalculo el path en cada frame.
            if (Path != null)
            {
                //Calculamos la dirección al siguiente nodo en el camino.
                nextNode = Path[0];
                Vector3 vecToCurrentTarget = (nextNode.transform.position - transform.position);

                transform.forward = vecToCurrentTarget.normalized;
                transform.position += transform.forward * pursueSpeed * Time.deltaTime;
            }
        }
        else
        {
            SwitchState(CommonState.attack);
        }
    }

    public override void End()
    {
        _anims.SetBool("Walking", false);
    }

    public void SetTarget(IDamageable<Damage, HitResult> subject)
    {
        Target = subject;
    }
    public void SetAttackRange(float range)
    {
        attackRange = range;
    }
}
