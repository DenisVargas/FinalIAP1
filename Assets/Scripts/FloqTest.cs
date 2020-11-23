using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using IA.Floquing;

public class FloqTest : MonoBehaviour
{
    public GameObject Lider = null;
    public LayerMask floquingAgentsLM = ~0;
    public LayerMask obstaclesAgents = ~0;
    public float _floquingRadius = 1f;
    public float _moveSpeed = 5f;
    public float _minSeparationRadius = 2;

    Vector3 Alligment = Vector3.zero;
    Vector3 cohesion = Vector3.zero;
    Vector3 separation = Vector3.zero;
    Vector3 avoidance = Vector3.zero;

    [Header("Weights")]
    [SerializeField] float SeparationWeight = 1f;
    [SerializeField] float cohetionWeight = 1f;
    [SerializeField] float avoidanceWeight = 1f;


    [Header("============= Debug =============")]
    public List<Transform> debugAllies = new List<Transform>();
    public List<Transform> obstacles = new List<Transform>();
    public Color debug_AlligmentColor = new Color();
    public Color debug_CohesionColor = new Color();
    public Color debug_SeparationColor = new Color();

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.matrix = Matrix4x4.Scale(new Vector3(1, 0, 1));
        Gizmos.DrawWireSphere(transform.position, _floquingRadius);

        Gizmos.color = debug_AlligmentColor;
        Gizmos.DrawLine(transform.position, transform.position + Alligment * 10);

        Gizmos.color = debug_CohesionColor;
        if (cohesion != Vector3.zero)
        {
            Gizmos.DrawLine(transform.position, transform.position + cohesion);
        }
        Gizmos.color = debug_SeparationColor;
        if (cohesion != Vector3.zero)
        {
            Gizmos.DrawLine(transform.position, transform.position + separation);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        debugAllies = getAllies();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 vecToLeader = (Lider.transform.position - transform.position);
        Vector3 dirToLeader = vecToLeader.normalized;
        float distToLeader = Vector3.Distance(transform.position, Lider.transform.position);

        var closerEntities = Physics.OverlapSphere(transform.position, _floquingRadius, floquingAgentsLM); //chequeo mi entorno.

        //vectores de floquing!!
        //debugAllies = getCloseAllies(closerEntities).ToList();
        obstacles = getCloserObstacles().ToList();

        Alligment = transform.getAlignment(debugAllies);
        cohesion = transform.getCohesion(debugAllies, cohetionWeight).YComponent(0);
        separation = transform.getSeparation(debugAllies, SeparationWeight).YComponent(0);
        avoidance = transform.getAvoidance(obstacles, avoidanceWeight).YComponent(0);

        //print("Avoidance is: " + avoidance);
        //print(separation);

        if (distToLeader > _floquingRadius)
        {
            Vector3 dirToMove = dirToLeader + separation + cohesion + avoidance;

            transform.forward = ((dirToMove + Alligment).normalized).YComponent(0);
            transform.position += (dirToMove * _moveSpeed * Time.deltaTime);
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
    /// <summary>
    /// Retorna los obstáculos más cercanos.
    /// </summary>
    public IEnumerable<Transform> getCloserObstacles()
    {
        var closerEntities = Physics.OverlapSphere(transform.position, _floquingRadius, obstaclesAgents); //chequeo mi entorno.

        var obstacles = closerEntities
                        .Select(x => x.transform);
        return obstacles;
    }
}
