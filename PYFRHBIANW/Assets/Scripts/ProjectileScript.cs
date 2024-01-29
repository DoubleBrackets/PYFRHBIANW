using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    private AttackInfo attackInfo;

    [SerializeField] private VFXScript impactVFX;

    private int playerId;

    public void InitializeProjectile(Vector3 direction, float speed, AttackInfo attackInfo, int playerId)
    {
        this.playerId = playerId;
        this.attackInfo = attackInfo;
        rb.velocity = direction * speed;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        transform.rotation = quaternion.LookRotation(direction, Vector3.up);
    }

    private void OnTriggerEnter(Collider other)
    {
        var combatEntity = other.gameObject.GetComponent<CombatEntity>();
        var info = attackInfo;
        info.direction = transform.forward;
        if (combatEntity != null)
        {
            var didTakeDamage = combatEntity.TakeDamage(info, 1 << playerId);
            if (didTakeDamage)
            {
                var vfx = Instantiate(impactVFX, transform.position, quaternion.identity);
                Destroy(gameObject);
            }
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Terrain"))
        {
            Destroy(gameObject);
        }
    }
}