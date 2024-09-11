using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPauseState : PlayerStateBase
{
    public override void Enter()
    {
        base.Enter();
        playerController.PlayAnimation("Idle");
    }

    public override void Update()
    {
        base.Update();
        if(!playerController.mouseOpen)
        {
            playerController.SwitchState(PlayerState.Idle);
        }
    }
}
