using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace MParmorLibrary;

public class
    JobGiver_AIFightMParmor_KeepDistanceFromMParmor_AntiMParmor : JobGiver_AIFightMParmor_KeepDistanceFromMParmor
{
    protected override Thing FindAttackTarget(Pawn pawn)
    {
        var list = new List<Thing>();
        var list2 = new List<Thing>();
        foreach (var item in ToolsLibrary_MParmorOnly.GetMParmor(pawn.Map))
        {
            if (needLOS && (!pawn.CanSee(item.ThingFixed) || needReach) &&
                !pawn.CanReach(item.ThingFixed, PathEndMode.Touch, Danger.Some))
            {
                continue;
            }

            list.Add(item.ThingFixed);
            if (item.HealthTracker.Shield <= 0f)
            {
                list2.Add(item.ThingFixed);
            }
        }

        if (list2.Any())
        {
            list2.SortBy(x => ToolsLibrary.GetDistance(x.Position, pawn.Position));
            return list2.First();
        }

        if (!list.Any())
        {
            return base.FindAttackTarget(pawn);
        }

        list.SortBy(x => ToolsLibrary.GetDistance(x.Position, pawn.Position));
        return list.First();
    }
}