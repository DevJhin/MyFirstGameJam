using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controller class for Player.
/// </summary>
public class PlayerController : FieldObjectController
{
    private Player player;

    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction attackAction;

    private readonly static string InputActionAssetPath = "InputSystem/InputSettings";

    public bool IsMoveButtonOnRepeat
    {
        get; private set;
    }

    public PlayerController(Player player)
    {
        this.player = player;

        //Load actions from InputActionAssset.
        var inputActionAsset = Resources.Load(InputActionAssetPath) as InputActionAsset;
        if (inputActionAsset == null)
        {
            Debug.LogError($"ResourceNotFoundError::Failed to load asset at '{InputActionAssetPath}'");
            return;
        }

        var playerActionMap = inputActionAsset.FindActionMap("Player");

        moveAction = playerActionMap.FindAction("Move");
        jumpAction = playerActionMap.FindAction("Jump");
        attackAction = playerActionMap.FindAction("Attack");

        //Input Binding �۾�.
        moveAction.performed += OnMoveActionPressed;
        moveAction.canceled += OnMoveActionReleased;
        jumpAction.performed += OnJumpAction;
        attackAction.performed += OnAttackAction;

        EnableInput();
    }


    public void OnPlayerUpdate()
    {


    }


    /// <summary>
    /// ��� Input�� ��Ȱ��ȭ �մϴ�.
    /// </summary>
    public void EnableInput()
    {
        moveAction.Enable();
        jumpAction.Enable();
        attackAction.Enable();
    }


    /// <summary>
    /// ��� Input�� Ȱ��ȭ�մϴ�.
    /// </summary>
    public void DisableInput()
    {
        moveAction.Disable();
        jumpAction.Disable();
        attackAction.Disable();
    }



    public bool TryGetMoveInput(out float value)
    {
        if (!IsMoveButtonOnRepeat)
        {
            value = default(float);
            return false;
        }

        value = moveAction.ReadValue<float>();

        return true;
    }


    /// <summary>
    /// MoveAction Press
    /// </summary>
    private void OnMoveActionPressed(InputAction.CallbackContext obj)
    {
        IsMoveButtonOnRepeat = true;
    }


    /// <summary>
    /// JumpAction�� ���� Binding �Լ�
    /// </summary>
    private void OnMoveActionReleased(InputAction.CallbackContext obj)
    {
        IsMoveButtonOnRepeat = false;
    }


    /// <summary>
    /// JumpAction�� ���� Binding �Լ�
    /// </summary>
    private void OnJumpAction(InputAction.CallbackContext obj)
    {
        player.Behavior.Jump();

        Debug.Log("Action 'Jump' Invoked!");
    }


    /// <summary>
    /// AttackAction�� ���� Binding �Լ�
    /// </summary>
    private void OnAttackAction(InputAction.CallbackContext obj)
    {
        player.Behavior.Attack();

        Debug.Log("Action 'Attack' Invoked!");
    }



}
