using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class InputManager : MonoBehaviour
{
    private const float InteractBufferSeconds = 0.2f;

    private Vector2 moveDirection = Vector2.zero;
    private bool interactPressed = false;
    private bool submitPressed = false;
    private int interactPressId = 0;
    private float lastInteractPressedTime = float.NegativeInfinity;

    private static InputManager instance;
    private Controls inputs;

    private void OnEnable()
    {
        if (inputs == null)
        {
            inputs = new Controls();
        }

        inputs.Enable();
    }

    private void OnDisable()
    {
        inputs.Disable();
    }

    private void Awake()
    {
        if (instance != null)
        {
            //Debug.LogError("Found more than one Input Manager in the scene.");
        }

        instance = this;
        inputs = new Controls();
    }

    public static InputManager GetInstance()
    {
        return instance;
    }

    // MovePressed misses diagonals in this setup, so we read the action directly.
    public void MovePressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            moveDirection = context.ReadValue<Vector2>();
            //moveDirection = inputs.Game.Move.ReadValue<Vector2>();
            ////Debug.Log("moveDirection performed: " + moveDirection);
        }
        else if (context.canceled)
        {
            moveDirection = context.ReadValue<Vector2>();
            //moveDirection = inputs.Game.Move.ReadValue<Vector2>();
            ////Debug.Log("moveDirection canceled: " + moveDirection);
        }
    }

    // Reads composite move input correctly.
    public void MovePressedControls()
    {
        moveDirection = inputs.Game.Move.ReadValue<Vector2>();
    }

    public Vector3 GetMoveDirection()
    {
        MovePressedControls();
        return moveDirection;
    }

    public void InteractButtonPressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            interactPressed = true;
            interactPressId++;
            lastInteractPressedTime = Time.unscaledTime;
            // //Debug.Log("press" + context);
        }
        else if (context.canceled)
        {
            interactPressed = false;
            // //Debug.Log("cancel press" + context);
        }
    }

    public void SubmitPressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            submitPressed = true;
        }
        else if (context.canceled)
        {
            submitPressed = false;
        }
    }

    // for any of the below 'Get' methods, if we're getting it then we're also using it,
    // which means we should set it to false so that it can't be used again until actually
    // pressed again.
    public bool GetInteractPressed()
    {
        return interactPressed;
    }

    public bool TryConsumeInteractPress(ref int lastProcessedPressId)
    {
        if (lastProcessedPressId == interactPressId)
        {
            return false;
        }

        if (Time.unscaledTime - lastInteractPressedTime > InteractBufferSeconds)
        {
            return false;
        }

        lastProcessedPressId = interactPressId;
        return true;
    }

    public bool GetSubmitPressed()
    {
        bool result = submitPressed;
        ////Debug.Log("submit pressed");
        submitPressed = false;
        return result;
    }

    public void RegisterSubmitPressed()
    {
        submitPressed = false;
    }
}
