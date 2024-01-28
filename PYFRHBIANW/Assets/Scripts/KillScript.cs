using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillScript : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        var combatEntity = other.collider.GetComponent<CombatEntity>();
        if (combatEntity != null)
        {
            combatEntity.Die();
        }
    }
}