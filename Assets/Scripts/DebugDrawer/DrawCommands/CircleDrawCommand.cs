using UnityEngine;


public class CircleDrawCommand : DrawCommand
{
    public Circle circle;
    public Color color;
    public bool drawAngleLine;

    public override void Execute()
    {
        GLUtils.DrawCircle(circle, color, drawAngleLine);
    }

}