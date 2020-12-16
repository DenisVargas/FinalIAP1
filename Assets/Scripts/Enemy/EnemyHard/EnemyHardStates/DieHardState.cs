using UnityEngine;

public class DieHardState<T> : State<T>
{
    EnemyHard _owner;
    Animator anims;

    public DieHardState(EnemyHard owner, Animator anim)
    {
        _owner = owner;
        anims = anim;
    }

    public override void Enter()
    {
        anims.SetBool("IsDying", true);
        _owner.DisableEntity();
    }
}
