using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackConfig : ScriptableObject
{
    [field: SerializeField] public AttackInfo Info { get; private set; }
}

[System.Serializable]
public struct AttackInfo
{
    public int damage;

    public float knockback;
    public bool stunning;
}