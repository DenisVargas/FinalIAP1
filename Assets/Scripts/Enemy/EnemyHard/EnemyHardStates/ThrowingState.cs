using UnityEngine;

public class ThrowingState<T> : State<T>
{
    public Transform _target;
    public EnemyHard _owner;
    public Animator _anims;


    public ThrowingState(EnemyHard enemy, Animator enemyHardAnim)
    {
        _anims = enemyHardAnim;
        _owner = enemy;
    }

    public override void Enter()
    {
        Debug.Log("===================== Trowing State ======================");
        _anims.SetBool("IsThrowing", true);
        _owner.isAttacking = true;
        _target = _owner.GetCurrentTarget();
    }
    public override void Update()
    {
        Debug.Log("Update");
        float distanceToTarget = Vector3.Distance(_owner.transform.position, _target.position);

        if (distanceToTarget > _owner.AttackRange)
            _owner.SM.Feed(EnemyHard.BE2_Inputs.IsNotInSigth);
    }

    public override void Exit()
    {
        _owner.isAttacking = false;
        _anims.SetBool("IsThrowing", false);
    }
}
