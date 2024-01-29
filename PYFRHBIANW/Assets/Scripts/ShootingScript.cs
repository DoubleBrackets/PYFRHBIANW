using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class ShootingScript : MonoBehaviour
{
    [SerializeField] private ThirdPersonInputProvider inputProvider;

    [SerializeField] private Transform fistGunBarrel;

    [SerializeField] private ProjectileScript fistProjectile;

    [SerializeField] private float cooldown;

    [SerializeField] private AttackConfig attackConfig;

    [SerializeField] private float projectileSpeed;

    [SerializeField] private AudioClip shootSFX;

    private int playerId;

    private float cooldownTimer;

    private void Awake()
    {
        inputProvider.OnTrigger.AddListener(OnShoot);
    }

    private void OnDestroy()
    {
        inputProvider.OnTrigger.RemoveListener(OnShoot);
    }

    private void Update()
    {
        cooldownTimer -= Time.deltaTime;
    }

    private void OnShoot(InputAction.CallbackContext arg0)
    {
        if (cooldownTimer > 0f || !gameObject.activeInHierarchy)
        {
            return;
        }

        SFXOneshotPlayer.Instance.PlaySFXOneshot(shootSFX);
        cooldownTimer = cooldown;
        var projectile = Instantiate(fistProjectile, fistGunBarrel.position, fistGunBarrel.rotation);
        projectile.InitializeProjectile(fistGunBarrel.forward, projectileSpeed, attackConfig.Info, playerId);
    }

    public void Initialize(int playerId)
    {
        this.playerId = playerId;
    }
}