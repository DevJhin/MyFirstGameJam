using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어의 행동 구현체
/// </summary>
public class PlayerBehavior : FieldObjectBehaviour{

    Player player;
    //private Vector3 groundRaycastGap;
    //private Vector3 sideRaycastGap; // 아래 colliderExtends로 대체
    private Vector2 colliderExtents; // 콜라이더 박스의 크기 (x,y 각각 콜라이더 size의 절반)
    private float airDragMultiplier;
    private float airDrag;
    private float jumpTimer;
    private bool facingLeft;
    private bool isMomentumXReset;

    // temp
    private Vector3 currentVelocity;

    public bool IsGroundedDebug => IsGrounded();

    public PlayerBehavior(Player player) { 
        this.player = player;
        Transform = player.transform;
        airDrag = 1 - player.airDrag;
        player.rb.gravityScale = player.gravityScale;
        colliderExtents = player.col.bounds.extents;
        //groundRaycastGap = new Vector3(player.col.size.x / 2, 0, 0);
        //sideRaycastGap = new Vector3(0, player.col.size.y / 2, 0);
    }

    public void OnPlayerUpdate() 
    {
        if (player.Controller.TryGetMoveInput(out float value))
        {
            Move(value);
        }
        else
		{
            currentVelocity.x = 0f;
        }
        UpdatePhysics();

        Transform.position += currentVelocity * Time.deltaTime;
    }

    //Physics Manager
    private void UpdatePhysics() {
        if (IsGrounded()) 
        {
            //Reset Multiplier
            airDragMultiplier = 1;
            currentVelocity.y = 0f;
            //Jump Check
            if (jumpTimer > Time.time) 
            {
                Jump();
            }
        }
        else {
            //Falling
            if (player.rb.velocity.y <= 0) 
            {
                // TODO: 낙하속도 자연스럽게 조정 필요 (현재는 낙하속도가 시간에 따라 변화하지 않음)
                currentVelocity.y += Physics2D.gravity.y * (player.fallMultiplier - 1) * Time.deltaTime;
                //player.rb.velocity += Physics2D.gravity * (player.fallMultiplier - 1) * Time.deltaTime;
            }
            //Releasing Jump Button During Jump
            else if (player.rb.velocity.y > 0 && !player.Controller.IsJumpButtonOnRepeat) 
            {
                currentVelocity.y += Physics2D.gravity.y * (player.lowJumpFallMultiplier - 1) * Time.deltaTime;
                //player.rb.velocity += Physics2D.gravity * (player.lowJumpFallMultiplier - 1) * Time.deltaTime;
            }
            //Air Drag Check
            if (!player.Controller.IsMoveButtonOnRepeat && !isMomentumXReset) {
                isMomentumXReset = true;
            }
            //Air Drag
            if (isMomentumXReset) {
                airDragMultiplier = airDrag;
            }
        }
    }

    //Movement
    public void Move(float value) 
    {
        currentVelocity.x = IsSideCollided() ? 
            0f : (value * player.moveSpeed * airDragMultiplier);

        if ((value < 0 && !facingLeft) || (value > 0 && facingLeft)) 
        {
            Flip();
        }
    }

    private void Flip() 
    {
        facingLeft = !facingLeft;
        Transform.rotation = Quaternion.Euler(0, facingLeft ? 180 : 0, 0);
    }

    //Jump
    public void CheckJump() {
        jumpTimer = Time.time + player.jumpCheckTimer;
    }

    // TODO: Rigidbody 조작 없이 점프 구현할 것
    private void Jump() {
        player.rb.velocity = Vector2.zero;
        //currentVelocity.y = player.jumpPower;
        Vector2 force = Vector2.up * player.jumpPower;
        player.rb.AddForce(force, ForceMode2D.Impulse);
        jumpTimer = 0;
        isMomentumXReset = false;
    }

    /// <summary>
    /// Player의 Attack BattleAction을 실행합니다.
    /// </summary>
    public void Attack() {
        player.AttackBattleAction.Execute(player);
    }

    //Ground Check
    private bool IsGrounded() {
        //Debug.DrawRay(Transform.position + groundRaycastGap, Vector2.down * player.raycastLength, Color.red, 3f);
        Debug.DrawRay(Transform.position, Vector2.down * (colliderExtents.y + player.raycastLength), Color.yellow, 3f);
        Debug.DrawRay(Transform.position + (Transform.right / 2), Vector2.down * (colliderExtents.y + player.raycastLength), Color.red, 3f);
        Debug.DrawRay(Transform.position - (Transform.right / 2), Vector2.down * (colliderExtents.y + player.raycastLength), Color.red, 3f);

        // TODO: 끝부분만 약간 땅에 닿는 경우는 낙하하도록 변경
        bool hit = Physics2D.BoxCast(player.col.bounds.center, player.col.size, 0f, Vector2.down, player.raycastLength, player.groundLayerMask);
        return hit;
    }

    //Side Check
    private bool IsSideCollided() {
        //Debug.DrawRay(Transform.position + sideRaycastGap, Transform.right * player.raycastLength, Color.red, 3f);
        //Debug.DrawRay(Transform.position - sideRaycastGap, Transform.right * player.raycastLength, Color.red, 3f);
        Debug.DrawRay(Transform.position + (Vector3.up * (colliderExtents.y / 2)), Transform.right * (colliderExtents.x + player.raycastLength), Color.green, 3f);
        Debug.DrawRay(Transform.position + (Vector3.down * (colliderExtents.y /2)), Transform.right * (colliderExtents.x + player.raycastLength), Color.green, 3f);

        bool hit = Physics2D.BoxCast(player.col.bounds.center, player.col.size, 0f, Transform.right, player.raycastLength, player.groundLayerMask);
        return hit;
    }
}