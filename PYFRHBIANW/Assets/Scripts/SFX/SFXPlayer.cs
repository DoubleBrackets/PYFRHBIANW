using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip clip;

    private void Start()
    {
        SFXOneshotPlayer.Instance.PlaySFXOneshot(clip);
    }
}