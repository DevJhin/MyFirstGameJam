using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어의 행동 구현체
/// </summary>
public class PlayerBehavior : FieldObjectBehaviour{

    Player player;
    private Vector3 groundRaycastGap;
    private Vector3 sideRaycastGap;
    private float airDragMultiplier;
    private float airDrag;
    private float jumpTimer;
    private bool facingLeft;
    private bool isMomentumXReset;

    public PlayerBehavior(Player player) { 
        this.player = player;
        Transform = player.transform;
        airDrag = 1 - player.airDrag;
        player.rb.gravityScale = player.gravityScale;
        groundRaycastGap = new Vector3(player.col.size.x / 2, 0, 0);
        sideRaycastGap = new Vector3(0, player.col.size.y / 2, 0);
    }

    public void OnPlayerUpdate() {
        if (player.Controller.TryGetMoveInput(out float value)) {
            Move(value);
        }
        UpdatePhysics();
    }

    //Physics Manager
    private void UpdatePhysics() {
        if (IsGrounded()) {
            //Reset Multiplier
            airDragMultiplier = 1;
            //Jump Check
            if (jumpTimer > Time.time) {
                Jump();
            }
        }
        else {
            //Falling
            if (player.rb.velocity.y < 0) {
                player.rb.velocity += Physics2D.gravity * (player.fallMultiplier - 1) * Time.deltaTime;
            }
            //Releasing Jump Button During Jump
            else if (player.rb.velocity.y > 0 && !player.Controller.IsJumpButtonOnRepeat) {
                player.rb.velocity += Physics2D.gravity * (player.lowJumpFallMultiplier - 1) * Time.deltaTime;
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
    public void Move(float value) {
        if (!IsSideCollided()) {
            Transform.position += new Vector3(value, 0, 0) * player.moveSpeed * airDragMultiplier * Time.deltaTime;
        }
        if ((value < 0 && !facingLeft) || (value > 0 && facingLeft)) {
            Flip();
        }
    }

    private void Flip() {
        facingLeft = !facingLeft;
        Transform.rotation = Quaternion.Euler(0, facingLeft ? 180 : 0, 0);
    }

    //Jump
    public void CheckJump() {
        jumpTimer = Time.time + player.jumpCheckTimer;
    }

    private void Jump() {
        player.rb.velocity = Vector2.zero;
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
        bool hit = Physics2D.Raycast(Transform.position + groundRaycastGap, Vector2.down, player.raycastLength, player.groundLayerMask) || Physics2D.Raycast(Transform.position - groundRaycastGap, Vector2.down, player.raycastLength, player.groundLayerMask);
        return hit;
    }

    //Side Check
    private bool IsSideCollided() {
        bool hit = Physics2D.Raycast(Transform.position + sideRaycastGap, Transform.right, player.raycastLength, player.groundLayerMask) || Physics2D.Raycast(Transform.position - sideRaycastGap, Transform.right, player.raycastLength, player.groundLayerMask);
        return hit;
    }
}