using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillScript : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        Debug.Log(gameObject.name);
        var combatEntity = other.collider.GetComponent<CombatEntity>();
        if (combatEntity != null)
        {
            combatEntity.Die();
        }
    }
}