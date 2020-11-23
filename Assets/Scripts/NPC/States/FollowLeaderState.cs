using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IA.FSM;

public class FollowLeaderState : State
{
    [SerializeField] Transform leader;

    public override void Begin()
    {
        base.Begin();
    }
    public override void Execute()
    {
        
    }
    public override void End()
    {
        base.End();
    }
}
