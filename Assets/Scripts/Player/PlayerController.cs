using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
//using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ��ҿ�����
/// </summary>
public class PlayerController : SingleMonoBase<PlayerController>, IStateMachineOwner
{
    //����ϵͳ
    public InputSystem inputSystem;

    //����ƶ�����
    public Vector2 inputMoveVec2;

    public PlayerModel playerModel;

    private StateMachine stateMachine;

    //���ܼ�ʱ��
    private float evadeTimer = 1f;

    //ת���ٶ�
    public float rotationSpeed = 8f;

    //���������Ϣ
    public PlayerConfig playerConfig;

    //���
    private List<PlayerModel> controllableModels;

    //��ǰ��ɫ���
    private int currentModelIndex;

    //���˱�ǩ�б�
    public List<string> enemyTagList;

    //�����¼�
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


        #region ���ɽ�ɫģ��
        for(int i = 0;i<playerConfig.models.Length;i++)
        {
            GameObject model = Instantiate(playerConfig.models[i], transform);
            controllableModels.Add(model.GetComponent<PlayerModel>());           
            controllableModels[i].gameObject.SetActive(false);
            //��ʼ����ɫģ��
            controllableModels[i].Init(enemyTagList);
            playerS.Add(controllableModels[i].gameObject);
        }
        #endregion

        #region �ٿ�����е�һ����ɫ
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
    /// �л�״̬
    /// </summary>
    /// <param name="playerState">״̬</param>
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
    /// �л���ɫ
    /// </summary>
    public void SwitchNextModel()
    {
        //ˢ��״̬��
        stateMachine.Clear();
        //�˳���ǰģ��
        playerModel.Exit();
        #region ������һ��ģ��
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
        //������һ��ģ��
        playerModel.Enter(prevPos, prevRot);
        //�л����볡״̬
        SwitchState(PlayerState.SwitchInNormal);
    }

    /// <summary>
    /// ���Ŷ���
    /// </summary>
    /// <param name="animationName">��������</param>
    /// <param name="fixedTransitionDuration">����ʱ��</param>
    public void PlayAnimation(string animationName,float fixedTransitionDuration = 0.25f)
    {
        playerModel.animator.CrossFadeInFixedTime(animationName, fixedTransitionDuration);
    }
    /// <summary>
    /// ���Ŷ���
    /// </summary>
    /// <param name="animationName">��������</param>
    /// <param name="fixedTransitionDuration">����ʱ��</param>
    /// <param name="fixedTimeOffset">������ʼ����ƫ��</param>
    public void PlayAnimation(string animationName, float fixedTransitionDuration,float fixedTimeOffset)
    {
        playerModel.animator.CrossFadeInFixedTime(animationName, fixedTransitionDuration,0,fixedTimeOffset);
    }

    private void Update()
    {
        //��������ƶ�����
        inputMoveVec2 = inputSystem.Player.Move.ReadValue<Vector2>().normalized;

        //LockMouse();

        if(inputSystem.Player.Mouse.triggered)
        {
            mouseOpen = !mouseOpen;
            if(!mouseOpen)
            {
                //����������м�
                Cursor.lockState = CursorLockMode.Locked;
                //���ع��
                Cursor.visible = false;
                //������ƶ�
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

        //�ָ����ܼ�ʱ��
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
            //����������м�
            Cursor.lockState = CursorLockMode.Locked;
            //���ع��
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

