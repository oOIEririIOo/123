using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    Idle,Idle_AFK ,Walk ,Run, RunEnd, TurnBack, Evade_Front, Evade_Front_End, Evade_Back, Evade_Back_End, NormalAttack, NormalAttackEnd, BigSkillStart, BigSkill, BigSkillEnd, SwitchInNormal,Pause
}

public class PlayerStateBase : StateBase
{
    
    //���״̬
    PlayerState state;
    //��ҿ�����
    protected PlayerController playerController;
    //���ģ��
    protected PlayerModel playerModel;
    //������Ϣ
    private AnimatorStateInfo stateInfo;
    //��¼��ǰ״̬�����ʱ��
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
        //ʩ������
        playerModel.characterController.Move(new Vector3(0, playerModel.gravity * Time.deltaTime, 0));

        if(playerController.mouseOpen)
        {
            playerController.SwitchState(PlayerState.Pause);
        }

        //״̬�����ʱ
        animationPlayTime += Time.deltaTime;

        #region ����ɫ�л�
        if(playerModel.currentState != PlayerState.BigSkillStart && playerModel.currentState != PlayerState.BigSkill 
            && (playerController.inputSystem.Player.SwitchDown.triggered || playerController.inputSystem.Player.SwitchUp.triggered))
        {
            //�л���ɫ
            playerController.SwitchNextModel();
        }
        #endregion
    }

    //��������
    public bool IsAnimationEnd()
    {
        #region �������Ž���
        //ˢ�¶���״̬��Ϣ
        stateInfo = playerModel.animator.GetCurrentAnimatorStateInfo(0);
        return (stateInfo.normalizedTime >= 1.0f && !playerModel.animator.IsInTransition(0));
        #endregion
    }

    public float NormalizedTime()
    {
        //ˢ�¶���״̬��Ϣ
        stateInfo = playerModel.animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.normalizedTime;
    }
}
