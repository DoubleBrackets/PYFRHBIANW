using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    public static ScoreUI Instance { get; private set; }

    [SerializeField] private TMP_Text text;

    private int[] deaths = new int[3];

    private void Awake()
    {
        Instance = this;
        UpdateText();
    }

    public void IncrementDeath(int playerId)
    {
        Debug.Log($"Player {playerId} died!");
        deaths[playerId]++;
        UpdateText();
    }

    private void UpdateText()
    {
        text.text = $"P1: {deaths[1]} Deaths\nP2: {deaths[2]} Deaths";
    }
}