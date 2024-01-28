using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Reset : MonoBehaviour
{
    [SerializeField] private string resetScene;
    
    [SerializeField] private InputAction resetAction;
    
    void Start()
    {
        resetAction.Enable();
        resetAction.performed += ResetActionOnPerformed;
    }

    private void OnDestroy()
    {
        resetAction.Disable();
        resetAction.performed -= ResetActionOnPerformed;
    }

    private void ResetActionOnPerformed(InputAction.CallbackContext obj)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(resetScene);
    }
}
