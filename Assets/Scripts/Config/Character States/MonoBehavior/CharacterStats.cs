using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;
using JetBrains.Annotations;

public class CharacterStats : MonoBehaviour
{

    public event Action<float, float> UpdateHealthBarOnAttack;

    public CharacterData_SO characterData;
    public CharacterData_SO templateData;
    public SkillConfig skillConfig;
    public SkillConfig templateSkillConfig;
    #region Read from data_SO

    private void Awake()
    {
        if(templateData != null)
        {
            characterData = Instantiate(templateData);
        }
        if(templateSkillConfig != null)
        {
            skillConfig = Instantiate(templateSkillConfig);
        }
    }
    public float MaxHealth
    {
        get { if (characterData != null) return characterData.maxHealth; else return 0; }
        set { characterData.maxHealth = value; }
    }

    public float CurrentHealth
    {
        get { if (characterData != null) return characterData.currentHealth; else return 0; }
        set { characterData.currentHealth = value; }
    }

    public float BaseDefence
    {
        get { if (characterData != null) return characterData.baseDefence; else return 0; }
        set { characterData.baseDefence = value; }
    }

    public float CurrentDefence
    {
        get { if (characterData != null) return characterData.currentDefence; else return 0; }
        set { characterData.currentDefence = value; }
    }
    #endregion

    #region Κά»χ
    public void TakeDamage(SkillConfig attacker)
    {
        float damage = attacker.normalAttackDamageMultiple[attacker.currentNormalAttackIndex - 1] * (1 - CurrentDefence*0.01f);
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);
        //TODO:Uppdate UI
        UpdateHealthBarOnAttack?.Invoke(CurrentHealth,MaxHealth);

    }
    #endregion

    #region Apply Data Change
    public void ApplyHealth(int amount)
    {
        if(CurrentHealth + amount <= MaxHealth)
        {
            CurrentHealth += amount;
        }
        else
        {
            CurrentHealth = MaxHealth;
        }
    }

    public void ATKBUFF(SkillConfig attacker,float ATKmultiple,float durationTime)
    {
        float[] attackMultiple = attacker.normalAttackDamageMultiple;
        for (int i = 0; i < attacker.normalAttackDamageMultiple.Length; i++)
        {
            attacker.normalAttackDamageMultiple[i] *= ATKmultiple;
        }
    }
    #endregion


}
