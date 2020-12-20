using System.Linq;
using UnityEngine;
using IA.LineOfSight;
using IA.FSM;
using System;

public class NPC : MonoBehaviour, IDamageable<Damage, HitResult>, IAgressor<Damage, HitResult>
{
    public Action<NPC> OnDie = delegate { };

    [SerializeField] protected int health = 100;
    [SerializeField] protected float AttackRange = 1.5f;
    [SerializeField] protected LineOfSightComponent sight;
    [SerializeField] protected Animator anims;
    [SerializeField] protected PathFindSolver solver;
    [SerializeField] protected FiniteStateMachine<CommonState> _states;

    public bool IsAlive => health > 0;

    public CommonState getCurrentStateType()
    {
        return _states.getCurrentStateType();
    }

    /// <summary>
    /// Esta unidad sufre daño.
    /// </summary>
    /// <param name="inputDamage">Estadísticas del daño recibido.</param>
    /// <returns>El resultado del ataque.</returns>
    public virtual HitResult getHit(Damage inputDamage)
    {
        health -= inputDamage.damageAmmount;

        if (health <= 0)
        {
            health = 0;
            _states.Feed(CommonState.dead);
        }

        return new HitResult();
    }
    /// <summary>
    /// Callback. Se llama cuando esta unidad realiza un ataque sobre otra entidad.
    /// </summary>
    /// <param name="hitResult">El resultado del ataque.</param>
    public virtual void onHit(HitResult hitResult)
    {
        //Si mi target actual está muerto, cambio de estado.
    }

    /// <summary>
    /// Encuentra y Evalúa si un Enemigo esta dentro de la línea de visión.
    /// </summary>
    protected IDamageable<Damage, HitResult> FindCloserTarget(string targetTag, float range, LayerMask filter)
    {
        Collider[] posibleTargets = Physics.OverlapSphere(transform.position, range, filter);
        //var closerTarget = posibleTargets
        //                   .Where(x => x.gameObject.CompareTag(targetTag))
        //                   .OrderBy(x => Vector3.Distance(transform.position, x.transform.position))
        //                   .Select(x => GetComponent<IDamageable<Damage, HitResult>>())
        //                   .FirstOrDefault();
        IDamageable<Damage, HitResult> closerTarget = null;
        Collider targetCollider = null;
        float distance = float.MaxValue;
        foreach (var col in posibleTargets)
        {
            if (col.CompareTag(targetTag))
            {
                float currentDistance = Vector3.Distance(col.transform.position, transform.position);
                if (currentDistance < distance)
                {
                    closerTarget = col.GetComponentInParent<IDamageable<Damage, HitResult>>();
                    targetCollider = col;
                }
            }
        }

        if(closerTarget != null && sight.IsInSight(targetCollider.transform))
            return closerTarget;

        return null;
    }
}
