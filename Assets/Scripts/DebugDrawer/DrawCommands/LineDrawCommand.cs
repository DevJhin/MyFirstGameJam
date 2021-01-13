using UnityEngine;


public class LineDrawCommand : DrawCommand
{
    public Vector2 start;
    public Vector2 end;
    public Color color;
    public bool asArrow;

    public override void Execute()
    {
        GLUtils.DrawLine(start, end, color, true);
    }
}