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
<<<<<<< HEAD
    Node _nextNode = null;           //El siguiente nodo al que nos moveremos.
    Node _originNode = null;         //El nodo en el que iniciamos el movimiento.
    Queue<Node> currentPath = new Queue<Node>();

    public bool debug_this = false;

    public void SetTarget(IDamageable<Damage, HitResult> subject)
    {
        Target = subject;
    }
    public void SetAttackRange(float range)
    {
        attackRange = range;
    }
=======
    Node nextNode = null;
>>>>>>> parent of 4bc7e22... Merge branch 'main' into Markitos

    public override void Begin()
    {
        _anims.SetBool("Walking", true);
    }

    public override void Execute()
    {
<<<<<<< HEAD
        if (debug_this)
        {
            print("Debugging");
        }

        if (Target.IsAlive)
        {
            Vector3 dir = (Target.transform.position - transform.position);
=======
        base.Execute();
>>>>>>> parent of 4bc7e22... Merge branch 'main' into Markitos

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
