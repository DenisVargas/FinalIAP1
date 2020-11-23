using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IA.FSM;

public class AlertState : State
{
    public override void Begin()
    {
        //Pongo mi forward como look up direction.
        base.Begin();
    }
    public override void End()
    {
        base.End();
    }

    void OnAlertAnimEnded()
    {
        //Al terminarse la animación hago algo.
    }
}
