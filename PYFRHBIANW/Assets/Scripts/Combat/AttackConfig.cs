using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackConfig", menuName = "AttackConfig")]
public class AttackConfig : ScriptableObject
{
    [field: SerializeField] public AttackInfo Info { get; private set; }
}

[System.Serializable]
public struct AttackInfo
{
    public int damage;

    public float knockback;
    public Vector3 direction;
    public bool stunning;
}