﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어의 행동 구현체
/// </summary>
public class PlayerBehavior : FieldObjectBehaviour{

    Player player;
    private bool facingRight = true;

    public PlayerBehavior(Player player)
    { 
        this.player = player;
        Transform = player.transform;

        player.rb.gravityScale *= player.jumpSpeed * player.jumpSpeed;
    }

    public void OnPlayerUpdate()
    {
        if (player.Controller.TryGetMoveInput(out float value))
        {
            Move(value);
        }
    }

    //Behavior Functions
    public void Move(float value) {
        Transform.position += new Vector3(value, 0, 0) * player.moveSpeed * Time.deltaTime;
        Debug.Log("Move");
        if ((value > 0 && !facingRight) || (value < 0 && facingRight)) {
            Flip();
        }
    }

    public void Jump() {
        if (isGrounded()) {
            player.rb.velocity = Vector2.zero;
            Vector2 force = Vector2.up * player.jumpPower * player.jumpSpeed;
            player.rb.AddForce(force, ForceMode2D.Impulse);
        }
    }

    public void Attack() {
        Debug.Log("Attack");
    }

    //Flip Object
    private void Flip() {
        facingRight = !facingRight;
        Transform.rotation = Quaternion.Euler(0, facingRight ? 0 : 180, 0);
    }

    //Ground Check
    private bool isGrounded() {
        Collider2D hit = Physics2D.OverlapBox(player.col.bounds.center, player.col.bounds.size, 0, player.groundLayerMask);
        return hit != null;
    }
}