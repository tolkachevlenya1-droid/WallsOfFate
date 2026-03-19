using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class InputManager : MonoBehaviour
{
    private Vector2 moveDirection = Vector2.zero;
    private bool interactPressed = false;
    private bool submitPressed = false;

    private static InputManager instance;
    private Controls inputs;

    private void OnEnable()
    {
        if(inputs == null) {
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

    //Работает странно с двойными векторами (считывает только значение по одному направлению)
    // Прищлось сделать отдельный метод MovePressedControls
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

    // Нормально обрабатывает двойные вектора    
    public void MovePressedControls()
    {
        moveDirection = inputs.Game.Move.ReadValue<Vector2>();
    }

    public Vector3 GetMoveDirection()
    {
        this.MovePressedControls();
        return moveDirection;
    }

    public void InteractButtonPressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            interactPressed = true;
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
        //Debug.Log("interact pressed" + interactPressed);
        bool result = interactPressed;
        interactPressed = false;
        return result;
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
