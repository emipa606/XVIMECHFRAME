using System.Collections.Generic;
using RimWorld;
using Verse;

namespace MParmorLibrary;

public class Verb_SortPawnsByDistance : Verb
{
    protected override bool TryCastShot()
    {
        var list = new List<ThingWithFloat>();
        foreach (var item in Caster.Map.mapPawns.AllPawnsSpawned)
        {
            var num =
                ((Caster.Position.ToVector3Shifted().x - item.Position.ToVector3Shifted().x) *
                 (Caster.Position.ToVector3Shifted().x - item.Position.ToVector3Shifted().x)) +
                ((Caster.Position.ToVector3Shifted().z - item.Position.ToVector3Shifted().z) *
                 (Caster.Position.ToVector3Shifted().z - item.Position.ToVector3Shifted().z));
            if (num <= EffectiveRange * EffectiveRange)
            {
                list.Add(new ThingWithFloat(item, num));
            }
        }

        list.Sort();
        for (var i = 0; i < list.Count - 1; i++)
        {
            FleckMaker.ConnectingLine(list[i].thing.DrawPos, list[i + 1].thing.DrawPos,
                FleckDefOf.XFMParmor_IceLaserLine, Caster.Map);
        }

        return true;
    }
}