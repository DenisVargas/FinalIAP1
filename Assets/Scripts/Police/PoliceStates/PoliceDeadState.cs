using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IA.FSM;

public class PoliceDeadState : State
{
    public override void Begin()
    {
        int deadAnim = Random.Range(1, 3);
        print("Entre a muerte");
        _anims.SetInteger("Dead", deadAnim); //Seteo la animacion.
    }

    public override void End()
    {
        _anims.SetInteger("Dead", 0); //Reseteo la animacion.
    }
}
