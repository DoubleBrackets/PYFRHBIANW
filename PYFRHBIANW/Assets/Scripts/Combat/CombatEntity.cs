using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class CombatEntity : MonoBehaviour
{
    [SerializeField] private int maxHealth;
    [SerializeField] private int currentHealth;
    [SerializeField] public int CurrentHealth => currentHealth;


    public int MaxHealth => maxHealth;

    public event Action<AttackInfo> OnDeath;
    public event Action<AttackInfo> OnHit;

    private int playerId;

    [Button("Die")]
    public void Die()
    {
        OnDeath?.Invoke(new AttackInfo());
    }

    public bool TakeDamage(AttackInfo info, int ignorePlayerMask)
    {
        if ((ignorePlayerMask & (1 << playerId)) != 0)
        {
            return false;
        }

        currentHealth -= info.damage;
        if (currentHealth <= 0)
        {
            OnDeath?.Invoke(info);
        }

        OnHit?.Invoke(info);
        return true;
    }

    public void Initialize(int playerId)
    {
        this.playerId = playerId;
    }
}