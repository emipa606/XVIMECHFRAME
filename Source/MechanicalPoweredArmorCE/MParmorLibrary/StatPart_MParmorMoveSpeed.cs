using System.Collections.Generic;
using RimWorld;
using Verse;

namespace MParmorLibrary;

public class StatPart_MParmorMoveSpeed : StatPart
{
    public override string ExplanationPart(StatRequest req)
    {
        if (IsWorking(req, out var def))
        {
            return "XFMParmor_MoveSpeedA".Translate() +
                   "XFMParmor_MoveSpeedB".Translate(def.MoveSpeed.ToString("0.##"));
        }

        return null;
    }

    public override void TransformValue(StatRequest req, ref float val)
    {
        if (IsWorking(req, out var def))
        {
            val = def.MoveSpeed;
        }
    }

    private static bool IsWorking(StatRequest req, out MParmorCore def)
    {
        def = null;
        if (req is not { HasThing: true, Thing: Pawn pawn })
        {
            return false;
        }

        IEnumerable<Apparel> enumerable = pawn.apparel?.WornApparel;
        foreach (var item in enumerable ?? [])
        {
            if (item is not MParmorCore mParmorCore)
            {
                continue;
            }

            def = mParmorCore;
            return true;
        }

        return false;
    }
}