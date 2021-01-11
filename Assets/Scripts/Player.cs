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

    [Header("MOVEMENT")]
    public float moveSpeed;
    public float airDrag;
    public float jumpPower;
    public float gravityScale;
    public float fallMultiplier;
    public float jumpCheckTimer;
    public float raycastLength;
    public Vector3 raycastGap;

    void Awake()
    {
        Behavior = new PlayerBehavior(this);
        Controller = new PlayerController(this);
    }

    private void Update()
    {
        Behavior.OnPlayerUpdate();
    }
}
