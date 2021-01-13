using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct Circle
{
    public Vector2 center;
    public float radius;
    public Vector2 forward;

    public Circle(Vector2 center, float radius, Vector2 forward)
    {
        this.center = center;
        this.radius = radius;
        this.forward = forward;

    }

}