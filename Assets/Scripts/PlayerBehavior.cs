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

    void Update() {
        Move();
        if (Input.GetKeyDown(KeyCode.W) && isGrounded()) {
            Jump();
        }
        if (Input.GetKeyDown(KeyCode.Space)) {
            Attack();
        }
    }

    //BEHAVIORS

    private void Move() {
        float movement = Input.GetAxis("Horizontal");
        transform.position += new Vector3(movement, 0, 0) * moveSpeed * Time.deltaTime;
    }

    private void Jump() {
        rb.velocity = Vector2.zero;
        Vector2 force = Vector2.up * jumpPower * jumpSpeed;
        rb.AddForce(force, ForceMode2D.Impulse);
    }

    private void Attack() {
        Debug.Log("Attack");
    }

    //GROUNDCHECK

    private bool isGrounded() {
        Collider2D hit = Physics2D.OverlapBox(col.bounds.center, col.bounds.size, 0, groundLayerMask);
        return hit != null;
    }
}