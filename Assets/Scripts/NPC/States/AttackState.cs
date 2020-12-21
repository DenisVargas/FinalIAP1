using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IA.FSM;
using System;

public class AttackState : State
{
    public event Action OnAttackEnded = delegate { };
    public Action<CommonState> swithStateTo = delegate { };
    public event Action<HitResult> OnApplyDamage = delegate { };
    public Func<IDamageable<Damage, HitResult>> getCurrentTarget = delegate{return null;};

    [SerializeField] float attackRange = 1.5f;
    [SerializeField] Damage damage = new Damage();
    [SerializeField] string[] Animations = new string[1] { "Attack" };

    #region DEBUGGING
    [Header("============ DEBUG ============")]
    [SerializeField] bool drawAttackRange = false;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.matrix = Matrix4x4.Scale(new Vector3(1, 0, 1));
        Gizmos.DrawWireSphere(transform.position, attackRange);
    } 
    #endregion

    public override void Begin()
    {
        damage.source = gameObject;

        if (Animations.Length == 0)
            Debug.LogWarning("El estado Attack no tiene definidas las animaciones.");
        if (Animations.Length == 1)
            _anims.Play(Animations[0]);
        else if(Animations.Length > 1)
        {
            int value = UnityEngine.Random.Range(0, Animations.Length);
            _anims.Play(Animations[value]);
        }

        var target = getCurrentTarget();
        Vector3 dirToTarget = (target.transform.position - transform.position).normalized;
        transform.forward = dirToTarget;
        Debug.Log("AttackState::START");
    }
    public override void End()
    {
        _anims.Play("Idle");
        Debug.Log("AttackState::END");
    }

    public void OnStartUp()
    {
        Debug.Log("AttackState::AnimationEvent::Startup");
    }

    public void OnActiveStart()
    {
        Debug.Log("AttackState::AnimationEvent::Active");
        var target = getCurrentTarget();
        if (target != null && target.IsAlive)
            OnApplyDamage(target.getHit(damage));
    }

    public void OnRecovery()
    {
        Debug.Log("AttackState::AnimationEvent::Recovery");
    }

    public void OnAttackEnd()
    {
        var target = getCurrentTarget();
        OnAttackEnded();
        Debug.Log("AttackState::AnimationEvent::EndoFAnimation");

        if (target == null)
            swithStateTo(CommonState.idle);

        if (target != null && !target.IsAlive)
            swithStateTo(CommonState.idle);
        else if (Animations.Length > 1)
        {
            int randomIndex = UnityEngine.Random.Range(0, 2);
            _anims.Play(Animations[randomIndex]);
        }
    }
}
