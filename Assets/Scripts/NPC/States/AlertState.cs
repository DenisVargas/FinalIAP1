using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IA.FSM;

public class AlertState : State
{
    public Action<CommonState> SwitchStateTo = delegate { };
    public Func<IDamageable<Damage, HitResult>> getCurrentAttackTarget = delegate { return null; };
    public Func<List<Zombie>> getAllies = delegate { return new List<Zombie>(); };

    public override void Begin()
    {
        //Pongo mi forward como look up direction.
        var Target = getCurrentAttackTarget();
        if (Target != null)
            transform.forward = ((Target.transform.position - transform.position).normalized).YComponent(0);

        _anims.Play("Alert");
    }

    public void OnAlertAnimEnded()
    {
        //Al terminarse la animación hago algo.
        SwitchStateTo(CommonState.pursue);
    }

    public void AlertTroops()
    {
        var allies = getAllies();
        var Target = getCurrentAttackTarget();
        foreach (var ally in allies)
        {
            if (ally == null) continue;
            ally.AlertUnit(Target);
        }
    }
}
