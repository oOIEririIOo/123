using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    Idle,Idle_AFK ,Walk ,Run, RunEnd, TurnBack, Evade_Front, Evade_Front_End, Evade_Back, Evade_Back_End, NormalAttack, NormalAttackEnd, BigSkillStart, BigSkill, BigSkillEnd, SwitchInNormal,Pause
}

public class PlayerStateBase : StateBase
{
    
    //玩家状态
    PlayerState state;
    //玩家控制器
    protected PlayerController playerController;
    //玩家模型
    protected PlayerModel playerModel;
    //动画信息
    private AnimatorStateInfo stateInfo;
    //记录当前状态进入的时间
    protected float animationPlayTime = 0f;


    public override void Init(IStateMachineOwner owner)
    {
        playerController = (PlayerController)owner;
        playerModel = playerController.playerModel;
    }
    public override void Enter()
    {
        animationPlayTime = 0f;
    }

    public override void Exit()
    {
        
    }

    public override void FixedUpdate()
    {
        
    }

    

    public override void LateUpdate()
    {
      
    }

    public override void UnInit()
    {

    }

    public override void Update()
    {
        //施加重力
        playerModel.characterController.Move(new Vector3(0, playerModel.gravity * Time.deltaTime, 0));

        if(playerController.mouseOpen)
        {
            playerController.SwitchState(PlayerState.Pause);
        }

        //状态进入计时
        animationPlayTime += Time.deltaTime;

        #region 检测角色切换
        if(playerModel.currentState != PlayerState.BigSkillStart && playerModel.currentState != PlayerState.BigSkill 
            && (playerController.inputSystem.Player.SwitchDown.triggered || playerController.inputSystem.Player.SwitchUp.triggered))
        {
            //切换角色
            playerController.SwitchNextModel();
        }
        #endregion
    }

    //动画结束
    public bool IsAnimationEnd()
    {
        #region 动画播放结束
        //刷新动画状态信息
        stateInfo = playerModel.animator.GetCurrentAnimatorStateInfo(0);
        return (stateInfo.normalizedTime >= 1.0f && !playerModel.animator.IsInTransition(0));
        #endregion
    }

    public float NormalizedTime()
    {
        //刷新动画状态信息
        stateInfo = playerModel.animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.normalizedTime;
    }
}
