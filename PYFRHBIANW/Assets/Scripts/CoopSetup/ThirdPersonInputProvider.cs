using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class ThirdPersonInputProvider : InputProvider, ThirdPersonActionMap.IGameplayActions
{
    // Unity Events
    public UnityEvent<InputAction.CallbackContext> OnMove;
    public UnityEvent<InputAction.CallbackContext> OnLook;
    public UnityEvent<InputAction.CallbackContext> OnJump;
    public UnityEvent<InputAction.CallbackContext> OnSprint;
    [FormerlySerializedAs("OnGrabThrow")] public UnityEvent<InputAction.CallbackContext> OnTrigger;

    protected override IInputActionCollection CreateMap()
    {
        return new ThirdPersonActionMap();
    }

    protected override void SetupMap()
    {
        (inputMap as ThirdPersonActionMap)?.Gameplay.SetCallbacks(this);
    }

    public void OnHorizontalMovement(InputAction.CallbackContext context)
    {
    }

    void ThirdPersonActionMap.IGameplayActions.OnMove(InputAction.CallbackContext context)
    {
        OnMove?.Invoke(context);
    }

    void ThirdPersonActionMap.IGameplayActions.OnLook(InputAction.CallbackContext context)
    {
        OnLook?.Invoke(context);
    }

    void ThirdPersonActionMap.IGameplayActions.OnJump(InputAction.CallbackContext context)
    {
        OnJump?.Invoke(context);
    }

    void ThirdPersonActionMap.IGameplayActions.OnSprint(InputAction.CallbackContext context)
    {
        OnSprint?.Invoke(context);
    }

    void ThirdPersonActionMap.IGameplayActions.OnTrigger(InputAction.CallbackContext context)
    {
        OnTrigger?.Invoke(context);
    }

    public void OnReset(InputAction.CallbackContext context)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}