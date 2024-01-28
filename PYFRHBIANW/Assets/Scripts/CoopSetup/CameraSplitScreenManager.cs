using System.Collections.Generic;
using UnityEngine;

public class CameraSplitScreenManager : MonoBehaviour
{
    public static CameraSplitScreenManager Instance { get; private set; }

    private readonly List<CoopCamera> cameras = new();

    private int displays;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Debug.Log("Displays connected: " + Display.displays.Length);
    }

    public void AddCamera(CoopCamera cam)
    {
        cameras.Add(cam);
        UpdateSplitScreen();
    }

    public void RemoveCamera(CoopCamera cam)
    {
        cameras.Remove(cam);
        UpdateSplitScreen();
    }

    private void UpdateSplitScreen()
    {
        // Sort in ascending player number
        cameras.Sort((a, b) => a.PlayerNumber.CompareTo(b.PlayerNumber));

        for (var i = 0; i < cameras.Count; i++)
        {
            // Debug.Log($"{cameras[i].PlayerNumber}");
        }

        // Use multiple displays if we have enough displays for ALL cameras
        var availableDisplays = Display.displays.Length;
        displays = availableDisplays;
        if (availableDisplays >= cameras.Count)
        {
            // Split screen on multiple displays
            for (var i = 0; i < cameras.Count; i++)
            {
                cameras[i].SetupMultipleDisplay(i);
                if (!Display.displays[i].active)
                {
                    Display.displays[i].Activate();
                }
            }
        }
        else
        {
            // Single display split screen
            for (var i = 0; i < cameras.Count; i++)
            {
                cameras[i].SetupSplitScreen(new Rect(i * 1f / cameras.Count, 0, 1f / cameras.Count, 1f));
            }
        }
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 100, 20), $"Displays: {displays}");
    }
}