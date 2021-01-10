using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

public class Player : FieldObject
{
    public PlayerBehavior Behavior
    {
        get; private set;
    }

    public PlayerController Controller
    {
        get; private set;
    }

    [Header("REFERENCES")]
    public LayerMask groundLayerMask;
    public Rigidbody2D rb;
    public BoxCollider2D col;

    [Header("MOVEMENT")]
    public float moveSpeed;
    public float jumpSpeed;
    public float jumpPower;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponentInChildren<BoxCollider2D>();

        Behavior = new PlayerBehavior(this);
        Controller = new PlayerController(this);
    }

    private void Update()
    {
        Behavior.OnPlayerUpdate();
    }
}
