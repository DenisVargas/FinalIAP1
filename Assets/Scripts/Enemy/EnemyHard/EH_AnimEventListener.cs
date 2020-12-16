using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EH_AnimEventListener : MonoBehaviour
{
    EnemyHard _owner;

    private void Awake()
    {
        _owner = GetComponentInParent<EnemyHard>();
    }

    //El combate siempre tiene 3 estados básicos: StartUp, Active, Recover
    void StartUp()
    {
        _owner.LookTowardsPlayer = true;
    }

    void Active()
    {
        _owner.Shoot();
        _owner.LookTowardsPlayer = false;
    }

    //void Recover()
    //{

    //}

    //void EndAttackAnimation()
    //{

    //}

    //Cuando termina el hit hacemos algo.
    void EndHitAnimation()
    {
        _owner.StartCoroutine(_owner.CriticalHitCoolDown());
        _owner.SM.Feed(EnemyHard.BE2_Inputs.NoTakingDamage);
    }
}
