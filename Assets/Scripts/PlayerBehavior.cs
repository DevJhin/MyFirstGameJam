using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour {

    [Header("REFERENCES")]
    [SerializeField]
    private LayerMask groundLayerMask;
    private Rigidbody2D rb;
    private BoxCollider2D col;
    [Header("MOVEMENT")]
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private float jumpSpeed;
    [SerializeField]
    private float jumpPower;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponentInChildren<BoxCollider2D>();
        rb.gravityScale *= jumpSpeed * jumpSpeed;
    }

    //BEHAVIORS

    public void Move() {
        /*float movement = Input.GetAxis("Horizontal");
        transform.position += new Vector3(movement, 0, 0) * moveSpeed * Time.deltaTime;*/
        Debug.Log("Move");
    }

    public void Jump() {
        if (isGrounded()) {
            rb.velocity = Vector2.zero;
            Vector2 force = Vector2.up * jumpPower * jumpSpeed;
            rb.AddForce(force, ForceMode2D.Impulse);
        }
    }

    public void Attack() {
        Debug.Log("Attack");
    }

    //GROUNDCHECK

    private bool isGrounded() {
        Collider2D hit = Physics2D.OverlapBox(col.bounds.center, col.bounds.size, 0, groundLayerMask);
        return hit != null;
    }
}