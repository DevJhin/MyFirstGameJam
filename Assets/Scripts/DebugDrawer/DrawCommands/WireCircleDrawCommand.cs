using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WireCircleDrawCommand : CircleDrawCommand
{
    public override void Execute()
    {
        GLUtils.DrawWireCircle(circle, color, drawAngleLine);
    }
}