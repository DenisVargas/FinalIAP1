using System;
using IA.FSM;
using UConsole = UnityEngine.MonoBehaviour;

[Serializable]
public class PoliceIddleState : State
{
    public PoliceIddleState()
    {
        stateType = CommonState.idle;
    }

    public override void Begin()
    {
        UConsole.print("Im in idle");
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
