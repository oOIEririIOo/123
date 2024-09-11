using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���������
/// </summary>
public class CameraManager : SingleMonoBase<CameraManager>
{
    //CM�������
    public CinemachineBrain cinemachineBrain;
    //�������
    public GameObject freeLookCamera;
    //����������
    public CinemachineFreeLook freeLook;

    private protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    //������������ӽ�
    public void ResetFreeLookCamera()
    {
        freeLook.m_YAxis.Value = 0.5f;
        freeLook.m_XAxis.Value = PlayerController.INSTANCE.playerModel.transform.eulerAngles.y;
    }
}
