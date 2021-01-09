using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controller class for Player.
/// </summary>
public class PlayerController
{
    private Player player;

    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction attackAction;

    private readonly static string InputActionAssetPath = "InputSystem/InputSettings";

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

        //Bind InputActions with PlayerController Methods.
        moveAction.performed += OnMoveAction;
        jumpAction.performed += OnJumpAction;
        attackAction.performed += OnAttackAction;

        EnableInput();
    }

    public void EnableInput()
    {
        moveAction.Enable();
        jumpAction.Enable();
        attackAction.Enable();
    }

    public void DisableInput()
    {
        moveAction.Enable();
        jumpAction.Enable();
        attackAction.Enable();
    }


    private void OnMoveAction(InputAction.CallbackContext obj)
    {
        float value = obj.ReadValue<float>();
        //player.Behavior.Move(axis);
        Debug.Log($"Action 'Move' Invoked!({value}) ");
    }

    private void OnJumpAction(InputAction.CallbackContext obj)
    {
        //player.Behavior.Jump();
        Debug.Log("Action 'Jump' Invoked!");
    }

    private void OnAttackAction(InputAction.CallbackContext obj)
    {
        //player.Behavior.Attack();
        Debug.Log("Action 'Attack' Invoked!");
    }



}
