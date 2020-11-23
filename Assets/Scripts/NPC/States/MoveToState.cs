using System;
using System.Collections.Generic;
using System.Linq;
using IA.FSM;
using IA.PathFinding;
using IA.LineOfSight;
using UnityEngine;

using UConsole = UnityEngine.MonoBehaviour;

[Serializable]
public class MoveToState : State
{
    public Action OnReachedTarget = delegate { };
    public Action findTarget = delegate { };
    public Func<List<Node>> getPathTo = delegate { return null; };

    [Header("Stats")]
    [SerializeField] float MoveSpeed = 3f;

    [Header("PathFinding")]
    [SerializeField] PathFindSolver _solver = null;
    [SerializeField] int _currentTargetNode = 0;
    [SerializeField] List<Node> path = new List<Node>();

    public MoveToState() { }

    public override void Begin()
    {
        UConsole.print("MoveState Start");
        _anims.SetBool("Walking", true);

        _solver.SetOrigin(transform.position);
        path = _solver.getPathWithSettings();
    }
    public override void Execute()
    {
        //UConsole.print("Moving to Position");

        Vector3 vecToCurrentTarget = (path[_currentTargetNode].transform.position - transform.position);
        Vector3 dir = vecToCurrentTarget.normalized;
        if (_currentTargetNode == (path.Count - 1))
        {
            if (vecToCurrentTarget.magnitude < _solver.ProximityTreshold)
                OnReachedTarget();
        }
        else
        if (vecToCurrentTarget.magnitude < _solver.ProximityTreshold)
        {
            _currentTargetNode++;

            vecToCurrentTarget = (path[_currentTargetNode].transform.position - transform.position);
            dir = vecToCurrentTarget.normalized;
        }

        //Calculamos la dirección al siguiente nodo en el camino.
        transform.forward = dir;
        transform.position += transform.forward * MoveSpeed * Time.deltaTime;

        findTarget();
    }

    public override void End()
    {
        UConsole.print("MoveState End");
        _anims.SetBool("Walking", false);
    }
}
