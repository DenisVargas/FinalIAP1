using System;
using IA.FSM;
using UnityEngine;
using UConsole = UnityEngine.MonoBehaviour;

[Serializable]
public class IdleState : State
{
    public Action CheckLineOfSight = delegate { };

    [SerializeField] bool debugThis = false;

    public IdleState()
    {
        stateType = CommonState.idle;
    }

    public override void Begin()
    {
        UConsole.print("Im in idle");
        _anims.Play("Idle");
    }

    public override void Execute()
    {
        if (debugThis)
            print("Debugging...");

        CheckLineOfSight();
    }
}
