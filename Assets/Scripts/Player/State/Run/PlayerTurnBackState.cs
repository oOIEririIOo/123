using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 180°转身状态
/// </summary>
public class PlayerTurnBackState : PlayerStateBase
{
    public override void Enter()
    {
        base.Enter();

        playerController.PlayAnimation("TurnBack", 0f);
    }

    public override void Update()
    {
        base.Update();

        #region 检测大招
        if (playerController.inputSystem.Player.BigSkill.triggered)
        {
            //进入大招状态
            playerController.SwitchState(PlayerState.BigSkillStart);
            return;
        }
        #endregion


        #region 是否播放结束
        if (IsAnimationEnd())
        {
            playerController.SwitchState(PlayerState.Walk);
        }
        #endregion
    }
}
