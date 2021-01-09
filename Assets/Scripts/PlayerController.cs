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

        //Input Binding �۾�.
        //Pressed ó�� �۵��ؾ� �ϴ� Move Input�� Binding���� �ʰ� OnUpdate���� ó���ؾ� ��.
        jumpAction.performed += OnJumpAction;
        attackAction.performed += OnAttackAction;
        
        //Since the limitation of Unity InputSytem, it does not have 'Pressed(Equivalent to Input.GetButton)' Input bindings.
        //Hence instead of binding function to "Move" Action, we need to check pressed state in Update function.
        //See https://forum.unity.com/threads/new-input-system-how-to-use-the-hold-interaction.605587/ for more information.

        EnableInput();
    }


    /// <summary>
    /// Pressed Input�� Ȯ���ϱ� ���� Update �Լ�.
    /// </summary>
    public void OnUpdate()
    {
        UpdateMoveAction();
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


    /// <summary>
    /// ���� Move ��ư ���¸� Ȯ���ϰ�, ������ �ִٸ�(Pressed) Move ���� ����.
    /// </summary>
    private void UpdateMoveAction()
    {
        float moveValue = moveAction.ReadValue<float>();
        if (moveValue > 0 || moveValue < 0)
        {
            player.Behavior.Move(moveValue);

            Debug.Log($"Action 'Move' Invoked!({moveValue}) ");
        }
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
