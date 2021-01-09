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
    void Awake()
    {
        Behavior = GetComponent<PlayerBehavior>();

        Controller = new PlayerController(this);

    }

}
