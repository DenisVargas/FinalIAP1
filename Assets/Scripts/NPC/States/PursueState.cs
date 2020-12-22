using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IA.FSM;
using IA.LineOfSight;
using IA.Floquing;
using IA.PathFinding;
#if UNITY_EDITOR
using UnityEditor; 
#endif

public class PursueState : State
{
    public Action<CommonState> SwitchState = delegate { };
    public Func<IDamageable<Damage, HitResult>> getCurrentTarget = delegate { return null; };

    [Header("Components")]
    [SerializeField] LineOfSightComponent _sight = null;

    [Header("Stats")]
    [SerializeField] float pursueSpeed = 5f;
    [SerializeField] float attackRange = 2f;

    [Header("Obstacle Avoidance Settings")]
    [SerializeField] LayerMask obstaclesAgents = ~0;
    [SerializeField] float _avoidanceAngle = 1f;
    [SerializeField] float _minimunAvoidanceRadius = 2f;
    [SerializeField] float _maximunAvoidanceRadius = 2f;
    [SerializeField] AnimationCurve AvoidanceWeightModifier = AnimationCurve.Linear(0, 0, 1, 1);

    /// <summary>
    /// Vector de Avoidance hacia obstáculos.
    /// </summary>
    Vector3 avoidance = Vector3.zero;

    //[Header("PathFinding")]
    //[SerializeField] PathFindSolver solver = null;

    //Esto es por si se mueve por nodos.
    IDamageable<Damage, HitResult> Target = null;
    //Node _nextNode = null;           //El siguiente nodo al que nos moveremos.
    //Node _originNode = null;         //El nodo en el que iniciamos el movimiento.
    //Queue<Node> currentPath = new Queue<Node>();

#if UNITY_EDITOR
    [Header("========== DEBUG ==============")]
    public bool debug_this = false;

    [Header("Avoidance")]
    [SerializeField] bool ShowAvoidance = false;
    [SerializeField] bool ShowMinAvoidanceRadius = false;
    [SerializeField] bool ShowMaxAvoidanceRadius = false;
    [SerializeField] Color debug_AvoidanceColorLabel = new Color();
    [SerializeField] Color debug_MinimunAvoidanceDistanceColor = new Color();
    [SerializeField] Color debug_MaximunAvoidanceDistanceColor = new Color();
    private void OnDrawGizmos()
    {
        if (ShowAvoidance)
        {
            Gizmos.color = debug_AvoidanceColorLabel;
            Gizmos.DrawLine(transform.position, transform.position + avoidance);

            Vector3 avoidance_medianPoint = getMedianPoint(transform.position, transform.position + avoidance);
            avoidance_medianPoint.y = +0.2f;

            DrawLabel(avoidance_medianPoint, $"Avoidance -> m: {avoidance.magnitude}", debug_AvoidanceColorLabel);

            if (ShowMinAvoidanceRadius)
            {
                //Rango
                Gizmos.color = debug_MinimunAvoidanceDistanceColor;
                Gizmos.matrix = Matrix4x4.Scale(new Vector3(1, 0, 1));
                Gizmos.DrawWireSphere(transform.position, _minimunAvoidanceRadius);

                var currentPosition = transform.position;

                //Ángulo
                Gizmos.color = debug_AvoidanceColorLabel;
                Gizmos.DrawLine(currentPosition, currentPosition + Quaternion.Euler(0, _avoidanceAngle + 1, 0) * transform.forward * _minimunAvoidanceRadius);
                Gizmos.DrawLine(currentPosition, currentPosition + Quaternion.Euler(0, -_avoidanceAngle - 1, 0) * transform.forward * _minimunAvoidanceRadius);
            }
            if (ShowMaxAvoidanceRadius)
            {
                Gizmos.color = debug_MaximunAvoidanceDistanceColor;
                Gizmos.matrix = Matrix4x4.Scale(new Vector3(1, 0, 1));
                Gizmos.DrawWireSphere(transform.position, _maximunAvoidanceRadius);
            }
        }
    }
    private Vector3 getMedianPoint(Vector3 from, Vector3 to)
    {
        return Vector3.Lerp(from, to, 0.5f);
    }
    private void DrawLabel(Vector3 labelPosition, string message, Color textColor)
    {
        GUIContent labelContent = new GUIContent();
        labelContent.text = message;
        GUIStyle style = new GUIStyle();
        style.normal.textColor = textColor;
        Handles.Label(labelPosition, labelContent, style);
    } 
#endif

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
#if UNITY_EDITOR
        if (debug_this)
            print("Debugging"); 
#endif

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

                var posibleObstacles = _sight.GetAllVisibles(transform.position, _maximunAvoidanceRadius, _avoidanceAngle, obstaclesAgents);
                avoidance = transform.getAvoidance(posibleObstacles, (position) => AvoidanceWeightModifier.Evaluate(position), _maximunAvoidanceRadius, _minimunAvoidanceRadius).YComponent(0);

                Vector3 movement = (dir.normalized + avoidance) * pursueSpeed * Time.deltaTime;
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

    //private void CalculateNOdePathToTarget()
    //{
    //    var currentCloserNode = solver.getCloserNode(transform.position);
    //    var closerTargetNode = solver.getCloserNode(Target.transform.position);
    //    var Path = solver.getPathTo(currentCloserNode, closerTargetNode);//Recalculo el path cada vez que completo el movimiento al siguiente nodo.

    //    currentPath = new Queue<Node>();
    //    foreach (var node in Path)
    //        currentPath.Enqueue(node);

    //    if (Path != null && Path.Count > 0)
    //    {
    //        //Seteamos las referencias.
    //        _originNode = currentPath.Dequeue();

    //        if (Path.Count > 0)
    //            _nextNode = currentPath.Dequeue();
    //    }
    //}
}
