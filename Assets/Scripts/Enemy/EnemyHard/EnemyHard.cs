using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHard : Enemy 
{
    public enum BE2_Inputs
    {
        IsInSigth,
        IsNotInSigth,
        TakingDamage,
        NoTakingDamage,
        IsDead
    }
    public FSM<BE2_Inputs> SM;

    public bool isAttacking = false;

    [Header("Combate")]
    public GameObject prefabProjectile = null;
    public Transform bulletSpawnPosition = null;


    [SerializeField] Collider HurtBox = null;
    [SerializeField] Collider MainCollider = null;

    //=================================== UNITY FUNCS ============================================

    protected override void Awake()
    {
        base.Awake(); //El Awake base rellena los componentes básicos.

        _agent.enabled = false;
        LookTowardsPlayer = false;

        //State Machine.
        var Iddle = new IddleHardState<BE2_Inputs>(this, _anims);
        var Throw = new ThrowingState<BE2_Inputs>(this, _anims);
        var GetHit = new TakeHardDamageState<BE2_Inputs>(this, _anims);
        var Die = new DieHardState<BE2_Inputs>(this, _anims);

        Iddle.AddTransition(BE2_Inputs.IsInSigth, Throw)
             .AddTransition(BE2_Inputs.TakingDamage, GetHit)
             .AddTransition(BE2_Inputs.IsDead, Die);

        Throw.AddTransition(BE2_Inputs.IsNotInSigth, Iddle)
             .AddTransition(BE2_Inputs.TakingDamage, GetHit)
             .AddTransition(BE2_Inputs.IsDead, Die);

        GetHit.AddTransition(BE2_Inputs.NoTakingDamage, Iddle)
              .AddTransition(BE2_Inputs.IsInSigth, Throw)
              .AddTransition(BE2_Inputs.IsDead, Die);

        SM = new FSM<BE2_Inputs>(Iddle);
    }

    protected override void Update()
    {
        if (_life <= 0)
        {
            SM.Feed(BE2_Inputs.IsDead);
            return;
        }

        base.Update();
        SM.Update();

        if (IsInSight(_target))
            SM.Feed(BE2_Inputs.IsInSigth);
    }

    public override void DisableEntity()
    {
        HurtBox.enabled = false;
        MainCollider.enabled = false;

        enabled = false;
    }

    //=================================== FUNCIONES MIEMBRO ======================================

    public void Shoot()
    {
        //Instancio un projectil.
        Bullet newbullet = Instantiate(prefabProjectile, bulletSpawnPosition.position, Quaternion.identity)
                          .GetComponent<Bullet>();
        Vector3 dirToTarget = (_target.transform.position - bulletSpawnPosition.position).normalized;
        newbullet.transform.forward = dirToTarget;

        newbullet.SetOwner(this);
        newbullet.Damage = _damage;
    }
    public Transform GetCurrentTarget()
    {
        if (_target)
            return _target.transform;
        else
            return null;
    }

    //=================================== Debugg Gizmos ==========================================

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
        var position = transform.position;

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(position, _range);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(position, position + Quaternion.Euler(0, _angle / 2, 0) * transform.forward * _range);
        Gizmos.DrawLine(position, position + Quaternion.Euler(0, -_angle / 2, 0) * transform.forward * _range);

        if (_target)
            Gizmos.DrawLine(position, _target.position);
    }

    //=================================== Sistema de Daño ========================================

    public override HitResult Hit(HitData hitData)
    {
        HitResult result = new HitResult();

        if (hitData.Damage > 0)
        {
            _life -= hitData.Damage;
            result.Conected = true;

            if (_life <= 0)
            {
                result.targetEliminated = true;
                GameProgressTracker.NotifyRangeEnemyKilled();
                SM.Feed(BE2_Inputs.IsDead);
            }
            else if (CanGetCriticalHit)
                SM.Feed(BE2_Inputs.TakingDamage);
        }

        return result;
    }

  public override HitData GetCombatStats()
    {
        return new HitData()
        {
            Damage = _damage
        };
    }
  
    public IEnumerator CriticalHitCoolDown()
    {
        CanGetCriticalHit = false;
        yield return new WaitForSeconds(CriticalhitCooldownTime);
        CanGetCriticalHit = true;
    }
}
