using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IA.FSM;
using System;

public class AttackState : State
{
    public event Action OnAttackEnded = delegate { };
    public event Action<HitResult> OnApplyDamage = delegate { };

    [SerializeField] float attackRange = 1.5f;
    [SerializeField] int damage = 10;
    [SerializeField] IDamageable<Damage, HitResult> _attackTarget = null;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.matrix = Matrix4x4.Scale(new Vector3(1, 0, 1));
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    public override void Begin()
    {
        _anims.SetBool("Attack", true);
        Debug.Log("AttackState::START");
    }
    public override void End()
    {
        _anims.SetBool("Attack", false);
        Debug.Log("AttackState::END");
    }

    /// <summary>
    /// Permite setear un target!
    /// </summary>
    /// <param name="attackTarget"></param>
    public void SetTarget(IDamageable<Damage, HitResult> attackTarget)
    {
        _attackTarget = attackTarget;
    }

    void AnimEvent_OnAttackEnded()
    {
        OnAttackEnded();
    }
    void AnimEvent_OnActivePhase()
    {
        if (_attackTarget != null)
            OnApplyDamage(_attackTarget.getHit(new Damage() { damageAmmount = damage }));
    }
}
