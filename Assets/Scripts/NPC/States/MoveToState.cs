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
    //On Reached Target Se ejecuta cuando llegamos al nodo de referencia.
    public Action OnReachedTarget = delegate { };
    //Utiliza Line Of sight para buscar Objetivos.
    public Action lookForTargets = delegate { };
    //Utiliza esta función para obtener un punto de referencia al cual dirigirse.
    public Func<Node> getEndTargetPoint = delegate { return null; };

    [Header("Stats")]
    [SerializeField] float MoveSpeed = 3f;

    [Header("PathFinding")]
    [SerializeField] PathFindSolver _solver = null;
    [SerializeField] int _currentTargetNode = 0;
    [SerializeField] List<Node> path = new List<Node>();

#if UNITY_EDITOR
    [Header("Debug")]
    [SerializeField] bool DebugThis = false; 
#endif

    public MoveToState() { }

    public override void Begin()
    {
        _anims.Play("Move");

        _solver.SetOrigin(transform.position);
        _solver.SetTarget(getEndTargetPoint());
        path = _solver.getPathWithSettings();
    }
    public override void Execute()
    {

        #if UNITY_EDITOR
        if (DebugThis)
            print("Debugging"); 
#endif

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

        lookForTargets();
    }
}
