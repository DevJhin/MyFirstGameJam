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

    //Gravity, Air Drag, Fall Updater
    private void UpdatePhysics() {
        if (isGrounded()) {
            player.rb.gravityScale = 0;
            airDragMultiplier = 1;
        }
        else {
            player.rb.gravityScale = player.gravityScale;
            //Fall
            if (player.rb.velocity.y < 0) {
                player.rb.gravityScale = player.gravityScale * player.fallMultiplier;
            }
            else if (!player.Controller.IsJumpButtonOnRepeat) {
                player.rb.gravityScale = player.gravityScale * player.fallMultiplier;
            }
            //Air Drag
            if (isMomentumXReset) {
                airDragMultiplier = airDrag;
            }
            if (!player.Controller.IsMoveButtonOnRepeat && !isMomentumXReset) {
                isMomentumXReset = true;
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
    public void Jump() {
        if (isGrounded()) {
            player.rb.velocity = Vector2.zero;
            Vector2 force = Vector2.up * player.jumpPower;
            player.rb.AddForce(force, ForceMode2D.Impulse);
            isMomentumXReset = false;
        }
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