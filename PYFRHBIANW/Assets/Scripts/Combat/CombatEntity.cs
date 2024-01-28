using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatEntity : MonoBehaviour
{
    [SerializeField] private int maxHealth;
    [SerializeField] private int currentHealth;
    [SerializeField] public int CurrentHealth => currentHealth;

    [SerializeField] private VFXScript deathVFX;

    public int MaxHealth => maxHealth;

    public event Action<AttackInfo> OnDeath;
    public event Action<AttackInfo> OnHit;

    private int playerId;

    public void TakeDamage(AttackInfo info, int ignorePlayerMask)
    {
        if ((ignorePlayerMask & (1 << playerId)) != 0)
        {
            return;
        }

        currentHealth -= info.damage;
        if (currentHealth <= 0)
        {
            OnDeath?.Invoke(info);
            Instantiate(deathVFX, transform.position, Quaternion.identity);
        }

        OnHit?.Invoke(info);
    }

    public void Initialize(int playerId)
    {
        this.playerId = playerId;
    }
}