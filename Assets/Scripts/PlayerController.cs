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

        //Input Binding 작업.
        //Pressed 처럼 작동해야 하는 Move Input은 Binding하지 않고 OnUpdate에서 처리해야 함.
        jumpAction.performed += OnJumpAction;
        attackAction.performed += OnAttackAction;
        
        //Since the limitation of Unity InputSytem, it does not have 'Pressed(Equivalent to Input.GetButton)' Input bindings.
        //Hence instead of binding function to "Move" Action, we need to check pressed state in Update function.
        //See https://forum.unity.com/threads/new-input-system-how-to-use-the-hold-interaction.605587/ for more information.

        EnableInput();
    }


    /// <summary>
    /// Pressed Input을 확인하기 위한 Update 함수.
    /// </summary>
    public void OnUpdate()
    {
        UpdateMoveAction();
    }


    /// <summary>
    /// 모든 Input을 비활성화 합니다.
    /// </summary>
    public void EnableInput()
    {
        moveAction.Enable();
        jumpAction.Enable();
        attackAction.Enable();
    }


    /// <summary>
    /// 모든 Input을 활성화합니다.
    /// </summary>
    public void DisableInput()
    {
        moveAction.Disable();
        jumpAction.Disable();
        attackAction.Disable();
    }


    /// <summary>
    /// 현재 Move 버튼 상태를 확인하고, 눌러져 있다면(Pressed) Move 동작 실행.
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
    /// JumpAction에 대한 Binding 함수
    /// </summary>
    private void OnJumpAction(InputAction.CallbackContext obj)
    {
        player.Behavior.Jump();

        Debug.Log("Action 'Jump' Invoked!");
    }


    /// <summary>
    /// AttackAction에 대한 Binding 함수
    /// </summary>
    private void OnAttackAction(InputAction.CallbackContext obj)
    {
        player.Behavior.Attack();

        Debug.Log("Action 'Attack' Invoked!");
    }



}
