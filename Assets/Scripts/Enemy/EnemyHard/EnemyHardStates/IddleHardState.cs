using UnityEngine;

public class IddleHardState<T> : State<T>
{
    public Animator Anims;
    public EnemyHard _owner;

    public IddleHardState(EnemyHard enemyHard, Animator enemyHardAnim)
    {
        Anims = enemyHardAnim;
        _owner = enemyHard;
    }
}
