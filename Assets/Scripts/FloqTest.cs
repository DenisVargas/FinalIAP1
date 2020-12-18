using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using IA.Floquing;
using IA.LineOfSight;
using System;

public class FloqTest : MonoBehaviour
{
    [SerializeField] LineOfSightComponent _sight = null;
    [SerializeField] float _minimunAvoidanceAngle = 1f;
    [SerializeField] float _minimunAvoidanceRadius = 2f;
    [SerializeField] float _maximunAvoidanceRadius = 2f;
    [SerializeField] AnimationCurve AvoidanceWeightModifier = AnimationCurve.Linear(0,0,1,1);

    [SerializeField] Color angleColor = Color.white;

    [SerializeField] GameObject Lider = null;
    [SerializeField] LayerMask floquingAgentsLM = ~0;
    [SerializeField] LayerMask obstaclesAgents = ~0;
    [SerializeField] float _floqRadius = 1f;
    [SerializeField] float _moveSpeed = 5f;

    Vector3 Alligment = Vector3.zero;
    Vector3 cohesion = Vector3.zero;
    /// <summary>
    /// Vector de Avoidance hacia mis aliados!
    /// </summary>
    Vector3 separation = Vector3.zero;
    /// <summary>
    /// Vector de Avoidance hacia obstáculos.
    /// </summary>
    Vector3 avoidance = Vector3.zero;
    Vector3 EndResult = Vector3.zero;

    [Header("Weights")]
    [SerializeField] float SeparationWeight = 1f;
    [SerializeField] float cohetionWeight = 1f;
    [SerializeField] float avoidanceWeight = 1f;

    [Header("============= Debug =============")]
    [SerializeField] List<Transform> debugAllies = new List<Transform>();
    [SerializeField] List<Transform> obstacles = new List<Transform>();

    [Header("Colors")]
    [SerializeField] Color debug_FlockingDistanceColor = new Color();
    [SerializeField] Color debug_AlligmentColor = new Color();
    [SerializeField] Color debug_CohesionColor = new Color();
    [SerializeField] Color debug_SeparationColor = new Color();
    [SerializeField] Color debug_LeaderDirectionColor = new Color();
    [SerializeField] Color debug_resultColor = new Color();

    [Header("Display Settings")]
    [SerializeField] bool ShowFlockRange = false;
    [SerializeField] bool StayQuiet = false; //Esto lo podemos usar para bloquear el movimiento y ver el resultado de los cálculos.
    [SerializeField] bool ShowAlligment = false;
    [SerializeField] bool ShowCohesion = false;
    [SerializeField] bool ShowSeparation = false;
    [SerializeField] bool ShowLeaderDirection = false;
    [SerializeField] bool ShowEndResult = false;

    [Header("Avoidance")]
    [SerializeField] bool ShowAvoidance = false;
    [SerializeField] bool ShowMinimumAvoidanceRadius = false;
    [SerializeField] bool ShowMaximunAvoidanceRadius = false;
    [SerializeField] Color debug_AvoidanceColorLabel = new Color();
    [SerializeField] Color debug_MinimunAvoidanceDistanceColor = new Color();
    [SerializeField] Color debug_MaximunAvoidanceDistanceColor = new Color();


    private void OnDrawGizmos()
    {
        if (ShowFlockRange)
        {
            Gizmos.color = debug_FlockingDistanceColor;
            Gizmos.matrix = Matrix4x4.Scale(new Vector3(1, 0, 1));
            Gizmos.DrawWireSphere(transform.position, _floqRadius); 
        }
        if (ShowLeaderDirection)
        {
            Gizmos.color = debug_LeaderDirectionColor;
            Gizmos.DrawLine(transform.position, Lider.transform.position);

            Vector3 Leader_labelPosition = getMedianPoint(transform.position, Lider.transform.position).YComponent(0 + 0.1f);
            DrawLabel(Leader_labelPosition, "Lider direction", Color.white);
        }

        if (ShowAlligment)
        {
            Gizmos.color = debug_AlligmentColor;
            Gizmos.DrawLine(transform.position, transform.position + Alligment);

            Vector3 Alligment_LabelPos = getMedianPoint(transform.position, transform.position + cohesion);
            Alligment_LabelPos.y += 0.1f;

            DrawLabel(Alligment_LabelPos, $"Alligment -> m: {Alligment.magnitude}", Color.white);
        }
        if (ShowCohesion)
        {
            Gizmos.color = debug_CohesionColor;
            Gizmos.DrawLine(transform.position, transform.position + cohesion);
            Vector3 cohesion_endPos = getMedianPoint(transform.position, transform.position + cohesion);
            cohesion_endPos.y += 0.1f;

            DrawLabel(cohesion_endPos, $"Cohesion -> m: {cohesion.magnitude}", Color.white);
        }
        if (ShowSeparation)
        {
            Gizmos.color = debug_SeparationColor;
            Gizmos.DrawLine(transform.position, transform.position + separation);

            Vector3 separation_medianPoint = getMedianPoint(transform.position, transform.position + separation);
            separation_medianPoint.y = +0.2f;

            DrawLabel(separation_medianPoint, $"Separation -> m: {separation.magnitude}", Color.white);
        }
        if (ShowAvoidance)
        {
            Gizmos.color = debug_AvoidanceColorLabel;
            Gizmos.DrawLine(transform.position, transform.position + avoidance);

            Vector3 avoidance_medianPoint = getMedianPoint(transform.position, transform.position + avoidance);
            avoidance_medianPoint.y = +0.2f;

            DrawLabel(avoidance_medianPoint, $"Avoidance -> m: {avoidance.magnitude}", debug_AvoidanceColorLabel);

            if (ShowMinimumAvoidanceRadius)
            {
                //Rango
                Gizmos.color = debug_MinimunAvoidanceDistanceColor;
                Gizmos.matrix = Matrix4x4.Scale(new Vector3(1, 0, 1));
                Gizmos.DrawWireSphere(transform.position, _minimunAvoidanceRadius);

                var currentPosition = transform.position;

                //Ángulo
                Gizmos.color = angleColor;
                Gizmos.DrawLine(currentPosition, currentPosition + Quaternion.Euler(0, _minimunAvoidanceAngle + 1, 0) * transform.forward * _minimunAvoidanceRadius);
                Gizmos.DrawLine(currentPosition, currentPosition + Quaternion.Euler(0, -_minimunAvoidanceAngle - 1, 0) * transform.forward * _minimunAvoidanceRadius);
            }
            if (ShowMaximunAvoidanceRadius)
            {
                Gizmos.color = debug_MaximunAvoidanceDistanceColor;
                Gizmos.matrix = Matrix4x4.Scale(new Vector3(1, 0, 1));
                Gizmos.DrawWireSphere(transform.position, _maximunAvoidanceRadius);
            }
        }

        if (ShowEndResult)
        {
            Gizmos.color = debug_resultColor;
            Gizmos.DrawLine(transform.position, transform.position + EndResult * 2);

            Vector3 Alligment_LabelPos = getMedianPoint(transform.position, transform.position + EndResult);
            Alligment_LabelPos.y += 0.1f;

            DrawLabel(Alligment_LabelPos, $"End Restult: {EndResult.magnitude}", Color.white);
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

    // Start is called before the first frame update
    void Start()
    {
        //debugAllies = getAllies();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 vecToLeader = (Lider.transform.position - transform.position);
        Vector3 dirToLeader = vecToLeader.normalized;
        float distToLeader = Vector3.Distance(transform.position, Lider.transform.position);

        obstacles = _sight.GetAllVisibles(transform.position, _maximunAvoidanceRadius, _minimunAvoidanceAngle, obstaclesAgents);

        Alligment = transform.getAlignment(debugAllies);
        cohesion = transform.getCohesion(debugAllies, cohetionWeight).YComponent(0);
        separation = transform.getSeparation(debugAllies, SeparationWeight).YComponent(0);
        avoidance = transform.getAvoidance(obstacles, (position) => AvoidanceWeightModifier.Evaluate(position), _maximunAvoidanceRadius, _minimunAvoidanceRadius).YComponent(0);

        //EndResult = dirToLeader + separation + cohesion + avoidance;
        EndResult = separation + cohesion + avoidance;

        if (StayQuiet)
            return;

        //La rotación depende del alligment solamente.

        if (distToLeader > _floqRadius)
        {
            //transform.forward = ((EndResult + Alligment).normalized).YComponent(0); //El componente forward debería ser algo que se actualiza en el tiempo.
            transform.forward = Vector3.Slerp(transform.forward, EndResult.normalized.YComponent(0), 0.5f);
            transform.position += (EndResult * _moveSpeed * Time.deltaTime);
        }

        //Buscar una forma de hacer que se detenga.
        if (distToLeader < 2)
        {
            //transform.forward = ((EndResult + Alligment).normalized).YComponent(0); //El componente forward debería ser algo que se actualiza en el tiempo.
            transform.forward = Vector3.Slerp(transform.forward, ((EndResult + Alligment).normalized).YComponent(0), 0.5f) ; //El componente forward debería ser algo que se actualiza en el tiempo.
        }
    }

    List<Transform> getAllies()
    {
        return FindObjectsOfType<FloqTest>()
            .Where(x => x != this)
            .Select(x => x.transform)
            .ToList();
    }

    /// <summary>
    /// Retorna los aliados mas cercanos para calcular el floquing.
    /// </summary>
    /// <returns>Una lista con los transforms de todos los aliados cercanos.</returns>
    public IEnumerable<Transform> getCloseAllies(Collider[] sceneInput)
    {
        var allies = sceneInput
                    .Where(x => x.GetComponent<FloqTest>() != null)
                    .Select(x => x.transform);

        return allies;
    }
}
