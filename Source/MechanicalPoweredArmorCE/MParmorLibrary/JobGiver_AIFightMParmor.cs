using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace MParmorLibrary;

public class JobGiver_AIFightMParmor : JobGiver_AIFightEnemies
{
    public bool needLOS;
    public bool needReach;

    public override ThinkNode DeepCopy(bool resolve = true)
    {
        var jobGiver_AIFightMParmor = (JobGiver_AIFightMParmor)base.DeepCopy(resolve);
        jobGiver_AIFightMParmor.needReach = needReach;
        jobGiver_AIFightMParmor.needLOS = needLOS;
        return jobGiver_AIFightMParmor;
    }

    protected override Thing FindAttackTarget(Pawn pawn)
    {
        var list = new List<Thing>();
        var list2 = new List<MParmorCore>();
        foreach (var item in ToolsLibrary_MParmorOnly.GetMParmor(pawn.Map))
        {
            if (needLOS && (!pawn.CanSee(item.ThingFixed) || needReach) &&
                !pawn.CanReachImmediate(item.ThingFixed, PathEndMode.Touch))
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
            return list2.First().ThingFixed;
        }

        if (!list.Any())
        {
            return base.FindAttackTarget(pawn);
        }

        {
            list.SortBy(x => ToolsLibrary.GetDistance(x.Position, pawn.Position));
            return list.First();
        }
    }
}