using System;
using IA.FSM;
using UConsole = UnityEngine.MonoBehaviour;

[Serializable]
public class IdleState : State
{
    public IdleState()
    {
        stateType = CommonState.idle;
    }

    public override void Begin()
    {
        UConsole.print("Im in idle");
        _anims.SetBool("idle", true);
    }
    public override void Execute()
    {
        UConsole.print("Staying in Idle");
    }
    public override void End()
    {
        UConsole.print("Im not longer in idle");
    }
}
