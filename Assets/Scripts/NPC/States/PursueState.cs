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

    public Action<CommonState> SwitchState = delegate { };
    public Func<IDamageable<Damage, HitResult>> getCurrentTarget = delegate { return null; };

    //Esto es por si se mueve por nodos.
    IDamageable<Damage, HitResult> Target = null;
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

    public override void Begin()
    {
        _anims.Play("Move");

        //Obtengo la referencia al target Actual.
        Target = getCurrentTarget();

        //Calculo el camino inicial, si me muevo por nodos.
    }

    public override void Execute()
    {
        if (debug_this)
        {
            print("Debugging");
        }

        if (Target.IsAlive)
        {
            Vector3 dir = (Target.transform.position - transform.position);

            if (dir.magnitude > attackRange)
            {
                //Esto es por si se mueve por nodos.
                //if (currentPath.Count == 0)
                //    CalculateNOdePathToTarget();

                //if (Move(_originNode, _nextNode, 0.1f)) //LLegamos al nextNode, y debemos recalcular nuestros siguiente paso.
                //    CalculateNOdePathToTarget();

                Vector3 movement = dir.normalized * pursueSpeed * Time.deltaTime;
                //Añadir obstacle avoidance.
                transform.forward = dir.normalized;
                transform.position += movement;
                return;
            }

            SwitchState(CommonState.attack);
        }
        else
        {
            //var enemy = checkForNearbyEnemiges();
            //if (enemy != null)
            //{
            //    Target = enemy;
            //    return;
            //}

            SwitchState(CommonState.idle);
        }
    }

    private void CalculateNOdePathToTarget()
    {
        var currentCloserNode = solver.getCloserNode(transform.position);
        var closerTargetNode = solver.getCloserNode(Target.transform.position);
        var Path = solver.getPathTo(currentCloserNode, closerTargetNode);//Recalculo el path cada vez que completo el movimiento al siguiente nodo.

        currentPath = new Queue<Node>();
        foreach (var node in Path)
            currentPath.Enqueue(node);

        if (Path != null && Path.Count > 0)
        {
            //Seteamos las referencias.
            _originNode = currentPath.Dequeue();

            if (Path.Count > 0)
                _nextNode = currentPath.Dequeue();
        }
    }
}
