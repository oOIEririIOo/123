using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��������
/// </summary>
[CreateAssetMenu(menuName = "Config/Skill")]
public class SkillConfig : ScriptableObject
{
    //��ǰ��ͨ��������
    public int currentNormalAttackIndex = 1;

    //��ͨ����ÿ�ε��˺�����
    public float[] normalAttackDamageMultiple;
}
