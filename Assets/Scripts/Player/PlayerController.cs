using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
//using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 玩家控制器
/// </summary>
public class PlayerController : SingleMonoBase<PlayerController>, IStateMachineOwner
{
    //输入系统
    public InputSystem inputSystem;

    //玩家移动输入
    public Vector2 inputMoveVec2;

    public PlayerModel playerModel;

    private StateMachine stateMachine;

    //闪避计时器
    private float evadeTimer = 1f;

    //转向速度
    public float rotationSpeed = 8f;

    //玩家配置信息
    public PlayerConfig playerConfig;

    //配队
    private List<PlayerModel> controllableModels;

    //当前角色编号
    private int currentModelIndex;

    //敌人标签列表
    public List<string> enemyTagList;

    //命中事件
    private Action<IHurt> onHitAction;

    public List<GameObject> playerS;

    public bool mouseOpen = false;

    protected private override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
        stateMachine = new StateMachine(this);
        inputSystem = new InputSystem();
        controllableModels = new List<PlayerModel>();


        #region 生成角色模型
        for(int i = 0;i<playerConfig.models.Length;i++)
        {
            GameObject model = Instantiate(playerConfig.models[i], transform);
            controllableModels.Add(model.GetComponent<PlayerModel>());           
            controllableModels[i].gameObject.SetActive(false);
            //初始化角色模型
            controllableModels[i].Init(enemyTagList);
            playerS.Add(controllableModels[i].gameObject);
        }
        #endregion

        #region 操控配队中第一个角色
        controllableModels[0].gameObject.SetActive(true);
        currentModelIndex = 0;
        playerModel = controllableModels[currentModelIndex];
        #endregion
    }
    private void Start()
    {
        //LockMouse();
        SwitchState(PlayerState.Idle);
        
    }


    /// <summary>
    /// 切换状态
    /// </summary>
    /// <param name="playerState">状态</param>
    public void SwitchState(PlayerState playerState)
    {
        playerModel.currentState = playerState;
        switch (playerState)
        {
            case PlayerState.Idle:
            case PlayerState.Idle_AFK:
                stateMachine.EnterState<PlayerIdleState>(true);
                break;
            case PlayerState.Run:
            case PlayerState.Walk:
                stateMachine.EnterState<PlayerRunState>(true);
                break;
            case PlayerState.RunEnd:
                stateMachine.EnterState<PlayerRunEndState>();
                break;
            case PlayerState.TurnBack:
                stateMachine.EnterState<PlayerTurnBackState>();
                break;
            case PlayerState.Evade_Front:
            case PlayerState.Evade_Back:
                if (evadeTimer != 1f) return;       
                stateMachine.EnterState<PlayerEvadeState>();
                evadeTimer -= 1f;          
                break;
            case PlayerState.Evade_Front_End:
            case PlayerState.Evade_Back_End:
                stateMachine.EnterState<PlayerEvadeEndState>();
                break;
            case PlayerState.NormalAttack:
                stateMachine.EnterState<PlayerNormalAttackState>(true);
                break;
            case PlayerState.NormalAttackEnd:
                stateMachine.EnterState<PlayerNormalAttackEndState>();
                break;
            case PlayerState.BigSkillStart:
                stateMachine.EnterState<PlayerBigSkillStartState>();
                break;
            case PlayerState.BigSkill:
                stateMachine.EnterState<PlayerBigSkillState>();
                break;
            case PlayerState.BigSkillEnd:
                stateMachine.EnterState<PlayerBigSkillEndState>();
                break;
            case PlayerState.SwitchInNormal:
                stateMachine.EnterState<PlayerSwitchInNoramlState>();
                break;
            case PlayerState.Pause:
                stateMachine.EnterState<PlayerPauseState>();
                break;

                
        }
    }

    /// <summary>
    /// 切换角色
    /// </summary>
    public void SwitchNextModel()
    {
        //刷新状态机
        stateMachine.Clear();
        //退出当前模型
        playerModel.Exit();
        #region 控制下一个模型
        currentModelIndex++;
            if(currentModelIndex >= controllableModels.Count)
            {
                currentModelIndex = 0;
            }
        PlayerModel nextmodel = controllableModels[currentModelIndex];
        nextmodel.gameObject.SetActive(true);
        Vector3 prevPos = playerModel.transform.position;
        Quaternion prevRot = playerModel.transform.rotation;
        playerModel = nextmodel;
        #endregion
        //进入下一个模型
        playerModel.Enter(prevPos, prevRot);
        //切换到入场状态
        SwitchState(PlayerState.SwitchInNormal);
    }

    /// <summary>
    /// 播放动画
    /// </summary>
    /// <param name="animationName">动画名称</param>
    /// <param name="fixedTransitionDuration">过渡时间</param>
    public void PlayAnimation(string animationName,float fixedTransitionDuration = 0.25f)
    {
        playerModel.animator.CrossFadeInFixedTime(animationName, fixedTransitionDuration);
    }
    /// <summary>
    /// 播放动画
    /// </summary>
    /// <param name="animationName">动画名称</param>
    /// <param name="fixedTransitionDuration">过渡时间</param>
    /// <param name="fixedTimeOffset">动画起始播放偏移</param>
    public void PlayAnimation(string animationName, float fixedTransitionDuration,float fixedTimeOffset)
    {
        playerModel.animator.CrossFadeInFixedTime(animationName, fixedTransitionDuration,0,fixedTimeOffset);
    }

    private void Update()
    {
        //更新玩家移动输入
        inputMoveVec2 = inputSystem.Player.Move.ReadValue<Vector2>().normalized;

        //LockMouse();

        if(inputSystem.Player.Mouse.triggered)
        {
            mouseOpen = !mouseOpen;
            if(!mouseOpen)
            {
                //锁定光标在中间
                Cursor.lockState = CursorLockMode.Locked;
                //隐藏光标
                Cursor.visible = false;
                //相机不移动
                CameraManager.INSTANCE.freeLook.enabled = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                CameraManager.INSTANCE.freeLook.enabled = false;
            }
        }

        if(inputSystem.Player.Menu.triggered && SceneManager.GetActiveScene().name != "Main")
        {
            SceneController.INSTANCE.TransitionToMain();
        }

        //恢复闪避计时器
        if(evadeTimer < 1f)
        {
            evadeTimer += Time.deltaTime;
            if(evadeTimer >1f)
            {
                evadeTimer = 1f;
            }
        }
    }
    /*
    private void LockMouse()
    {
        if (SceneManager.GetActiveScene().name != "Main")
        {
            //锁定光标在中间
            Cursor.lockState = CursorLockMode.Locked;
            //隐藏光标
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

    }
    */
    private void OnEnable()
    {
        inputSystem.Enable();
    }

    private void OnDisable()
    {
        inputSystem.Disable();
    }
}

