using System.Collections.Generic;
using RimWorld;
using Verse;

namespace MParmorLibrary;

public class Verb_Chain : Verb
{
    public static readonly List<Thing> targetsChain = [];

    protected override bool TryCastShot()
    {
        var list = new List<Thing>();
        list.AddRange(Caster.Map.mapPawns.AllPawnsSpawned);
        list.Remove(Caster);
        targetsChain.Add(Caster);
        Chain(Caster, list);
        for (var i = 0; i < targetsChain.Count - 1; i++)
        {
            FleckMaker.ConnectingLine(targetsChain[i].DrawPos, targetsChain[i + 1].DrawPos,
                FleckDefOf.XFMParmor_IceLaserLine, Caster.Map);
        }

        targetsChain.Clear();
        return true;
    }

    protected virtual void Chain(Thing origin, List<Thing> primedTargets)
    {
        ThingWithFloat thingWithFloat = null;
        foreach (var primedTarget in primedTargets)
        {
            var distance = ToolsLibrary.GetDistance(origin.Position, primedTarget.Position);
            if (!(distance <= EffectiveRange * EffectiveRange) ||
                !GenSight.LineOfSight(origin.Position, primedTarget.Position, Caster.Map))
            {
                continue;
            }

            if (thingWithFloat == null)
            {
                thingWithFloat = new ThingWithFloat(primedTarget, distance);
            }
            else if (thingWithFloat.floatValue > distance)
            {
                thingWithFloat = new ThingWithFloat(primedTarget, distance);
            }
        }

        if (thingWithFloat == null)
        {
            return;
        }

        targetsChain.Add(thingWithFloat.thing);
        primedTargets.Remove(thingWithFloat.thing);
        Chain(thingWithFloat.thing, primedTargets);
    }
}