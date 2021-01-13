using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectorDrawCommand : CircleDrawCommand
{
    public float angle;

    public override void Execute()
    {
        GLUtils.DrawSector(circle, angle, color, drawAngleLine);
    }

}
