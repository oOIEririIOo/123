using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 180��ת��״̬
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

        #region ������
        if (playerController.inputSystem.Player.BigSkill.triggered)
        {
            //�������״̬
            playerController.SwitchState(PlayerState.BigSkillStart);
            return;
        }
        #endregion


        #region �Ƿ񲥷Ž���
        if (IsAnimationEnd())
        {
            playerController.SwitchState(PlayerState.Walk);
        }
        #endregion
    }
}
