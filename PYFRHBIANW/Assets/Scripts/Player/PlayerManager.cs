using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private CoopCamera coopCamera;
    [SerializeField] private ThirdPersonCamera thirdPersonCamera;
    [SerializeField] private Transform thirdPersonCameraTarget;
    [SerializeField] private MovementController movementController;
    [SerializeField] private CombatEntity combatEntity;

    [SerializeField] private ThirdPersonInputProvider inputProvider;

    private Vector2 lookInput;
    private Vector2 moveInput;

    public enum PlayerState
    {
        Idle,
        Moving,
        Jumping,
        Airborne,
        Stunned,
        Dead
    }

    public void Setup(int playerNumber)
    {
        coopCamera.Initialize(playerNumber);
        combatEntity.Initialize(playerNumber);
    }

    private void Awake()
    {
        inputProvider.OnLook.AddListener(UpdateLookInput);
        inputProvider.OnMove.AddListener(UpdateMoveInput);
        inputProvider.OnJump.AddListener(TryJump);
    }

    private void OnDestroy()
    {
        inputProvider.OnLook.RemoveListener(UpdateLookInput);
        inputProvider.OnMove.RemoveListener(UpdateMoveInput);
        inputProvider.OnJump.RemoveListener(TryJump);
    }

    private void TryJump(InputAction.CallbackContext context)
    {
        Debug.Log("JUMP");
        movementController.TryStartJump();
    }

    private void UpdateLookInput(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    private void UpdateMoveInput(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }


    private void Update()
    {
        // third person camera
        thirdPersonCamera.UpdateCamera(lookInput, Time.deltaTime);
    }

    private void FixedUpdate()
    {
        // movement
        movementController.UpdateMovement(moveInput, Time.fixedDeltaTime, thirdPersonCameraTarget.transform);
    }
}