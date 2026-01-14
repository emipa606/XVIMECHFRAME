using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace MParmorLibrary;

public class JobGiver_AIFightMParmor_FollowSecond : JobGiver_AIFightEnemiesFollowSecocnd
{
    public bool needLOS;
    public bool needReach;

    public override ThinkNode DeepCopy(bool resolve = true)
    {
        var jobGiver_AIFightMParmor_FollowSecond = (JobGiver_AIFightMParmor_FollowSecond)base.DeepCopy(resolve);
        jobGiver_AIFightMParmor_FollowSecond.needReach = needReach;
        jobGiver_AIFightMParmor_FollowSecond.needLOS = needLOS;
        return jobGiver_AIFightMParmor_FollowSecond;
    }

    protected override Thing FindAttackTarget(Pawn pawn)
    {
        var list = new List<Thing>();
        foreach (var item in ToolsLibrary_MParmorOnly.GetMParmor(pawn.Map))
        {
            if (!needLOS || pawn.CanSee(item.ThingFixed) && !needReach ||
                pawn.CanReach(item.ThingFixed, PathEndMode.Touch, Danger.Some))
            {
                list.Add(item.ThingFixed);
            }
        }

        if (!list.Any())
        {
            return base.FindAttackTarget(pawn);
        }

        list.SortBy(x => ToolsLibrary.GetDistance(x.Position, pawn.Position));
        return list.First();
    }
}