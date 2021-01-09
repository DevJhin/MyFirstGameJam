using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior{

    Player player;

    private Transform transform;

    public PlayerBehavior(Player player)
    { 
        this.player = player;
        transform = player.transform;

        player.rb.gravityScale *= player.jumpSpeed * player.jumpSpeed;
    }
   
    
    //Behavior Functions
    
    public void Move(float value) {
        transform.position += new Vector3(value, 0, 0) * player.moveSpeed * Time.deltaTime;
        Debug.Log("Move");
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

    //Ground Check
    private bool isGrounded() {
        Collider2D hit = Physics2D.OverlapBox(player.col.bounds.center, player.col.bounds.size, 0, player.groundLayerMask);
        return hit != null;
    }
}