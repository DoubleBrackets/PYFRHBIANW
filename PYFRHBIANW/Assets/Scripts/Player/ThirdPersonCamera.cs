using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [SerializeField] private Transform cameraTarget;

    [SerializeField] private float sensitivity;

    private float rotX;
    private float rotY;

    public void UpdateCamera(Vector2 input, float deltaTime)
    {
        rotX += input.y * sensitivity * deltaTime;
        rotY += input.x * sensitivity * deltaTime;

        rotX = Mathf.Clamp(rotX, -80, 80);

        cameraTarget.rotation = Quaternion.Euler(rotX, rotY, 0);
    }
}