using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace MParmorLibrary;

public class JobGiver_AIFightMParmor_FollowSecond_AntiShield : JobGiver_AIFightMParmor_FollowSecond
{
    protected override Thing FindAttackTarget(Pawn pawn)
    {
        var list = new List<Thing>();
        var list2 = new List<MParmorCore>();
        foreach (var item in ToolsLibrary_MParmorOnly.GetMParmor(pawn.Map))
        {
            if (needLOS && (!pawn.CanSee(item.ThingFixed) || needReach) &&
                !pawn.CanReach(item.ThingFixed, PathEndMode.Touch, Danger.Some))
            {
                continue;
            }

            list.Add(item.ThingFixed);
            if (item.HealthTracker.Shield > 0f)
            {
                list2.Add(item);
            }
        }

        if (list2.Any())
        {
            list2.SortBy(x => 0f - x.HealthTracker.Shield);
            return list2.First().Wearer;
        }

        if (!list.Any())
        {
            return base.FindAttackTarget(pawn);
        }

        list.SortBy(x => ToolsLibrary.GetDistance(x.Position, pawn.Position));
        return list.First();
    }
}