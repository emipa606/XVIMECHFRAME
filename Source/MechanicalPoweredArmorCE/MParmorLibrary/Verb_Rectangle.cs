using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace MParmorLibrary;

public class Verb_Rectangle : Verb
{
    public override void DrawHighlight(LocalTargetInfo target)
    {
        base.DrawHighlight(target);
        if (!(target != null))
        {
            return;
        }

        _ = target.CenterVector3;

        var list = new List<IntVec3>();
        foreach (var item in ToolsLibrary.GetCellsInRect(Caster.Position, target.Cell, 5))
        {
            if (ToolsLibrary.GetDistanceBetweenLineAndPoint(Caster.Position.ToVector3Shifted(),
                    target.Cell.ToVector3Shifted(), item.ToVector3Shifted()) <= 25f)
            {
                list.Add(item);
            }
        }

        GenDraw.DrawFieldEdges(ToolsLibrary.GetCellsInRect(Caster.Position, target.Cell, 5),
            new Color(1f, 0f, 0f, 0.75f));
        GenDraw.DrawFieldEdges(list, new Color(0.30980393f, 83f / 85f, 0.9372549f, 0.75f));
    }

    protected override bool TryCastShot()
    {
        return true;
    }
}