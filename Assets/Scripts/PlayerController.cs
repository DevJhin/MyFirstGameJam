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


    /// <summary>
    /// 현재 Move 버튼이 Repeat(꾹 누르고 있는) 상태인가?
    /// </summary>
    public bool IsMoveButtonOnRepeat
    {
        get; private set;
    }


    /// <summary>
    /// 현재 Jump 버튼이 Repeat(꾹 누르고 있는) 상태인가?
    /// </summary>
    public bool IsJumpButtonOnRepeat
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

        //Input Binding 작업.
        moveAction.performed += OnMoveActionPressed;
        moveAction.canceled += OnMoveActionReleased;

        jumpAction.performed += OnJumpActionPressed;
        jumpAction.canceled += OnJumpActionReleased;
        
        attackAction.performed += OnAttackAction;

        EnableInput();
    }


    public void OnPlayerUpdate()
    {


    }


    /// <summary>
    /// 모든 Input을 활성화 합니다.
    /// </summary>
    public void EnableInput()
    {
        moveAction.Enable();
        jumpAction.Enable();
        attackAction.Enable();
    }


    /// <summary>
    /// 모든 Input을 비활성화합니다.
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
    /// MoveAction의 Press에 대한 Binding 함수
    /// </summary>
    private void OnMoveActionPressed(InputAction.CallbackContext obj)
    {
        IsMoveButtonOnRepeat = true;
    }


    /// <summary>
    /// MoveAction의 Relese에 대한 Binding 함수
    /// </summary>
    private void OnMoveActionReleased(InputAction.CallbackContext obj)
    {
        IsMoveButtonOnRepeat = false;
    }


    /// <summary>
    /// JumpAction의 Pressed에 대한 Binding 함수
    /// </summary>
    private void OnJumpActionPressed(InputAction.CallbackContext obj)
    {
        IsJumpButtonOnRepeat = true;
        Debug.Log("Action 'Jump' Pressed!");
        player.Behavior.CheckJump();
    }


    /// <summary>
    /// JumpAction의 Release에 대한 Binding 함수
    /// </summary>
    private void OnJumpActionReleased(InputAction.CallbackContext obj)
    {
        IsJumpButtonOnRepeat = false;
        Debug.Log("Action 'Jump' Released!");
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
