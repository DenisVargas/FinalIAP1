using UnityEngine;

public class TakeHardDamageState<T> : State<T>
{
    public Animator anims;
    public EnemyHard _owner;

    public TakeHardDamageState(EnemyHard enemyHard, Animator enemyHardAnim)
    {
        anims = enemyHardAnim;
        _owner = enemyHard;
    }

    public override void Enter()
    {
        Debug.Log("Enemigo recibe daño");
        anims.SetBool("ReceiveDamage", true);
        _owner.CanGetCriticalHit = false;
    }
    public override void Exit()
    {
        anims.SetBool("ReceiveDamage", false);
    }
}
