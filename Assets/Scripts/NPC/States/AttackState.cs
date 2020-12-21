using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IA.FSM;
using System;

public class AttackState : State
{
    [SerializeField] private AudioSource ManagerSounds;
    [SerializeField] private AudioClip AttackSoundS;
    public event Action OnAttackEnded = delegate { };
    public event Action<HitResult> OnApplyDamage = delegate { };

    [SerializeField] float attackRange = 1.5f;
<<<<<<< HEAD
    [SerializeField] Damage damage = new Damage();
    [SerializeField] string[] Animations = new string[1] { "Attack" };
=======
    [SerializeField] int damage = 10;
    [SerializeField] IDamageable<Damage, HitResult> _attackTarget = null;
>>>>>>> parent of 4bc7e22... Merge branch 'main' into Markitos

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.matrix = Matrix4x4.Scale(new Vector3(1, 0, 1));
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    public override void Begin()
    {
<<<<<<< HEAD
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
=======
        _anims.SetBool("Attack", true);
>>>>>>> parent of 4bc7e22... Merge branch 'main' into Markitos
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
<<<<<<< HEAD
        Debug.Log("AttackState::AnimationEvent::Active");
        var target = getCurrentTarget();
        if (target != null && target.IsAlive)
            OnApplyDamage(target.getHit(damage));
        AttackSound();
=======
        _attackTarget = attackTarget;
>>>>>>> parent of 4bc7e22... Merge branch 'main' into Markitos
    }

    void AnimEvent_OnAttackEnded()
    {
        OnAttackEnded();
    }
    void AnimEvent_OnActivePhase()
    {
<<<<<<< HEAD
        var target = getCurrentTarget();
        OnAttackEnded();
        Debug.Log("AttackState::AnimationEvent::EndoFAnimation");

        if (target != null && !target.IsAlive)
            swithStateTo(CommonState.idle);
        else if (Animations.Length > 1)
        {
            int randomIndex = UnityEngine.Random.Range(0, 2);
            _anims.Play(Animations[randomIndex]);
        }
    }

    void AttackSound()
    {
        ManagerSounds.PlayOneShot(AttackSoundS);
=======
        if (_attackTarget != null)
            OnApplyDamage(_attackTarget.getHit(new Damage() { damageAmmount = damage }));
>>>>>>> parent of 4bc7e22... Merge branch 'main' into Markitos
    }

}
