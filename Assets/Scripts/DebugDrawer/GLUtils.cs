using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Custom GL Library.
/// </summary>
public static class GLUtils
{
    static Material m_Internal_Mateial;
    static Material InternalMaterial
    {
        get
        {
            if (!m_Internal_Mateial)
            {
                // Unity has a built-in shader that is useful for drawing
                // simple colored things.
                Shader shader = Shader.Find("Hidden/Internal-Colored");
                m_Internal_Mateial = new Material(shader);
                m_Internal_Mateial.hideFlags = HideFlags.HideAndDontSave;
                // Turn on alpha blending
                m_Internal_Mateial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                m_Internal_Mateial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                // Turn backface culling off
                m_Internal_Mateial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
                // Turn off depth writes
                m_Internal_Mateial.SetInt("_ZWrite", 0);
            }
            return m_Internal_Mateial;
        }
    }

    /// <summary>
    /// Draw a circle
    /// </summary>
    /// <param name="circle"></param>
    /// <param name="color">Color of circle</param>
    /// <param name="drawArrow">Draw an arrow that shows the direction of circle?</param>
    /// <param name="precision">Segment number of the circle. Bigger number results in higher quality and lower performance.</param>
    public static void DrawCircle(Circle circle, Color color, bool drawArrow = true, int precision = 30)
    {
        InternalMaterial.SetPass(0);

        float delta = Mathf.PI * 2 / precision;
        float angle = 0;
        // Draw Triangles
        GL.Begin(GL.TRIANGLES);
        GL.Color(color);
        for (int i = 0; i < precision; ++i)
        {
            GL.Vertex(circle.center);
            GL.Vertex3(Mathf.Cos(angle) * circle.radius + circle.center.x, Mathf.Sin(angle) * circle.radius + circle.center.y, 0);
            angle += delta;
            GL.Vertex3(Mathf.Cos(angle) * circle.radius + circle.center.x, Mathf.Sin(angle) * circle.radius + circle.center.y, 0);
        }
        GL.End();

        if (drawArrow) DrawLine(circle.center, circle.center + circle.forward * circle.radius, Color.white, true);
    }

    /// <summary>
    /// Draw a circle
    /// </summary>
    /// <param name="circle"></param>
    /// <param name="color">Color of circle</param>
    /// <param name="drawArrow">Draw an arrow that shows the direction of circle?</param>
    /// <param name="precision">Segment number of the circle. Bigger number results in higher quality and lower performance.</param>
    public static void DrawWireCircle(Circle circle, Color color, bool drawArrow = true, int precision = 30)
    {
        InternalMaterial.SetPass(0);

        float delta = Mathf.PI * 2 / precision;
        float angle = 0;
        // Draw Triangles
        GL.Begin(GL.LINES);
        GL.Color(color);
        for (int i = 0; i < precision; ++i)
        {
            GL.Vertex3(Mathf.Cos(angle) * circle.radius + circle.center.x, Mathf.Sin(angle) * circle.radius + circle.center.y, 0);
            angle += delta;
            GL.Vertex3(Mathf.Cos(angle) * circle.radius + circle.center.x, Mathf.Sin(angle) * circle.radius + circle.center.y, 0);
        }
        GL.End();

        if (drawArrow) DrawLine(circle.center, circle.center + circle.forward * circle.radius, Color.white, true);
    }


    /// <summary>
    /// Draw a circle
    /// </summary>
    /// <param name="circle"></param>
    /// <param name="color">Color of circle</param>
    /// <param name="drawArrow">Draw an arrow that shows the direction of circle?</param>
    /// <param name="precision">Segment number of the circle. Bigger number results in higher quality and lower performance.</param>
    public static void DrawSector(Circle circle, float angle, Color color, bool drawArrow = true, int precision = 30)
    {
        InternalMaterial.SetPass(0);

        float radAngle = Mathf.Deg2Rad * angle;

        float delta = radAngle / precision;
        float tempAngle = Angle360(Vector2.right, circle.forward)*Mathf.Deg2Rad - radAngle * 0.5f;
        // Draw Triangles
        GL.Begin(GL.TRIANGLES);
        GL.Color(color);
        for (int i = 0; i < precision; ++i)
        {
            GL.Vertex(circle.center);
            GL.Vertex3(Mathf.Cos(tempAngle) * circle.radius + circle.center.x, Mathf.Sin(tempAngle) * circle.radius + circle.center.y, 0);
            tempAngle += delta;
            GL.Vertex3(Mathf.Cos(tempAngle) * circle.radius + circle.center.x, Mathf.Sin(tempAngle) * circle.radius + circle.center.y, 0);
        }
        GL.End();

        if (drawArrow) DrawLine(circle.center, circle.center + circle.forward * circle.radius, Color.white, true);
    }


    /// <summary>
    /// Draws an arrow that points direction of circle
    /// </summary>
    /// <param name="radius"></param>
    public static void DrawLine(Vector2 start, Vector2 end, Color color, bool asArrow)
    {
        InternalMaterial.SetPass(0);

        GL.Begin(GL.LINES);
        GL.Color(color);

        //Draw body segment
        GL.Vertex(start);
        GL.Vertex(end);

        if (asArrow)
        {
            //Draw left segment
            GL.Vertex(start);
            GL.Vertex(end);

            //Draw right segment
            GL.Vertex(start);
            GL.Vertex(end);
        }

        GL.End();
    }

    private static float Angle360(Vector2 from, Vector2 to)
    {
        Vector2 direction = to - from;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        if (angle < 0f) angle += 360f;

        return angle;
    }
}