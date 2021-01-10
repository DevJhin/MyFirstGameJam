using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어의 행동 구현체
/// </summary>
public class PlayerBehavior : FieldObjectBehaviour{

    Player player;
    private float airDragMultiplier;
    private float airDrag;
    private float jumpTimer;
    private bool facingLeft;
    private bool isMomentumXReset;

    public PlayerBehavior(Player player) { 
        this.player = player;
        Transform = player.transform;
        airDrag = 1 - player.airDrag;
    }

    public void OnPlayerUpdate() {
        if (player.Controller.TryGetMoveInput(out float value)) {
            Move(value);
        }
        UpdatePhysics();
    }

    //Physics Manager
    private void UpdatePhysics() {
        if (isGrounded()) {
            //Reset Physics
            player.rb.gravityScale = 0;
            airDragMultiplier = 1;
            //Jump Check
            if (jumpTimer > Time.time) {
                Jump();
            }
        }
        else {
            //Mid-Air
            player.rb.gravityScale = player.gravityScale;
            //Falling
            if (player.rb.velocity.y < 0) {
                player.rb.gravityScale = player.gravityScale * player.fallMultiplier;
            }
            //Holding Jump Button
            else if (!player.Controller.IsJumpButtonOnRepeat) {
                player.rb.gravityScale = player.gravityScale * player.fallMultiplier;
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
        Transform.position += new Vector3(value, 0, 0) * player.moveSpeed * airDragMultiplier * Time.deltaTime;
        Debug.Log("Moving");
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

    //Attack
    public void Attack() {
        Debug.Log("Attack");
    }

    //Ground Check
    private bool isGrounded() {
        Collider2D hit = Physics2D.OverlapBox(player.col.bounds.center, player.col.bounds.size, 0, player.groundLayerMask);
        return hit != null;
    }
}