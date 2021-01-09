using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

public class Player : MonoBehaviour
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
        //InputSystem이 Pressed 상태에서의 Binding을 지원하지 않기 때문에 
        //어쩔 수 없이 Update함수 안에서 버튼 입력 상태를 확인해야 하는데..
        //이것 때문에 Controller 클래스를 MonoBehaviour로 만드는 것 보다는 
        //일단은 확실하게 MonoBehaviour로 합의된 Player에서 Controller를 Update 시켜주는 방식으로 구현. 
        Controller.OnUpdate();
    }
}
