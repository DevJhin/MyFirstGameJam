using UnityEngine;


public class GLDebug
{
    private static readonly Color DEFAULT_CIRCLE_COLOR = new Color(0, 1, 0, 0.02f);

    public static void DrawCircle(Circle circle, Color color, float time = 0f)
    {
        var cmd = new CircleDrawCommand
        {
            circle = circle,
            color = color,
            LifeTime = time
        };
        GLCommandDispatcher.Instance.Publish(cmd);
    }


    public static void DrawWireCircle(Circle circle, Color color, float time = 0f)
    {
        var cmd = new WireCircleDrawCommand
        {
            circle = circle,
            color = color,
            LifeTime = time
        };
        GLCommandDispatcher.Instance.Publish(cmd);
    }


    public static void DrawSector(Circle circle, float angle, Color color, float time = 0f)
    {
        var cmd = new SectorDrawCommand
        {
            circle = circle,
            angle = angle,
            color = color,
            LifeTime = time,
            
        };
        GLCommandDispatcher.Instance.Publish(cmd);
    }

    public static void DrawLine(Vector2 start, Vector2 end, Color color, bool asArrow = false, float time = 0f)
    {
        var cmd = new LineDrawCommand
        {
            start = start,
            end = end,
            color = color,
            asArrow = asArrow,
            LifeTime = time
        };

        GLCommandDispatcher.Instance.Publish(cmd);
    }

}