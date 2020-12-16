using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
public class Enemy : MonoBehaviour, IFighter<HitData, HitResult>
{
    //Stats Básicas
    [SerializeField] protected float _life = 0;
    [SerializeField] protected int _damage = 0;

    [Header("Line Of Sight")]
    [SerializeField] protected float _range = 0;
    [SerializeField] protected float _angle = 0;
    [SerializeField] protected LayerMask visibles = ~0;

    [Header("Cooldowns & Timers")]
    [SerializeField] protected float CriticalhitCooldownTime = 1f;
    [HideInInspector] public bool CanGetCriticalHit = true;

    [Header("Combate")]
    public float AttackRange = 4;

    public bool LookTowardsPlayer { get; set; } = false;

    //Componentes de Unity.
    protected Transform _target;
    protected Animator _anims;
    protected NavMeshAgent _agent;

    //=================================== Unity Funcs ============================================

    protected virtual void Awake()
    {
        //Componentes
        _target = FindObjectOfType<Player>().transform;
        _anims = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
    }
    protected virtual void Update()
    {
        if (LookTowardsPlayer && _target != null)
        {
            Vector3 lookDirection = (_target.position - transform.position);
            lookDirection.y = 0;
            transform.forward = lookDirection.normalized;
        }
    }


    //=================================== Custom Funcs ===========================================

    /// <summary>
    /// Callback que se llama cuando el Enemigo entra en la fase de Recovery.
    /// Usa esto para chequear condiciones para encadenar un nuevo ataque o pasar a otro estado.
    /// </summary>
    public virtual void EvaluateTarget() { }
    /// <summary>
    /// Callback que se llama cuando el Enemigo termina su animación de ataque.
    /// Usa esto para ejecutar la acción correspondiente de acuerdo a lo evaluado.
    /// </summary>
    public virtual void ExecuteEvaluateAction() { }
    /// <summary>
    /// Deshabilita esta unidad, pero no lo destruye del mundo.
    /// </summary>
    public virtual void DisableEntity() { enabled = false; }

    public virtual void StopNavmeshNavigation()
    {
        _agent.isStopped = true;
    }
    public virtual void SetNavmeshDestination(Vector3 location)
    {
        if (_agent.isStopped) _agent.isStopped = false;
        _agent.SetDestination(location);
    }

    //=================================== Line of Sight ==========================================

    protected bool IsInSight(Transform target)
    {
        var positionDiference = target.position - transform.position;
        var distance = positionDiference.magnitude;
        var angleToTarget = Vector3.Angle(transform.forward, positionDiference);

        if (distance < _range && angleToTarget < (_angle / 2))
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(transform.position + Vector3.up, positionDiference.normalized, out hitInfo, _range, visibles))
                return hitInfo.transform == target;
        }

        return false;
    }

    //=================================== Sistema de Daño ========================================

    public bool IsAlive => _life > 0;

    public virtual HitData GetCombatStats() { return new HitData() { Damage = 0 }; }

    public virtual HitResult Hit(HitData hitData)
    { return new HitResult() { Conected = false, targetEliminated = false }; }

    public virtual void OnHiConnected(HitResult hitResult) { }

    //=================================== Debugg Gizmos ==========================================

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.matrix = Matrix4x4.Scale(new Vector3(1, 0, 1));
        Gizmos.DrawWireSphere(transform.position, AttackRange);
    }
}
