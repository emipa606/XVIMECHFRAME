using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace MParmorLibrary;

public class JobGiver_AIFightMParmor_KeepDistanceFromMParmor : JobGiver_AIFightEnemies_KeepDistanceFromMParmor
{
    private readonly List<Thing> targets = [];
    public bool needLOS;

    public bool needReach;

    public override ThinkNode DeepCopy(bool resolve = true)
    {
        var jobGiver_AIFightMParmor_KeepDistanceFromMParmor =
            (JobGiver_AIFightMParmor_KeepDistanceFromMParmor)base.DeepCopy(resolve);
        jobGiver_AIFightMParmor_KeepDistanceFromMParmor.needReach = needReach;
        jobGiver_AIFightMParmor_KeepDistanceFromMParmor.needLOS = needLOS;
        return jobGiver_AIFightMParmor_KeepDistanceFromMParmor;
    }

    protected override Thing FindAttackTarget(Pawn pawn)
    {
        targets.Clear();
        foreach (var item in ToolsLibrary_MParmorOnly.GetMParmor(pawn.Map))
        {
            var thingFixed = item.ThingFixed;
            if (!needLOS || pawn.CanSee(thingFixed) && !needReach ||
                pawn.CanReach(thingFixed, PathEndMode.Touch, Danger.Some))
            {
                targets.Add(thingFixed);
            }
        }

        if (!targets.Any())
        {
            return base.FindAttackTarget(pawn);
        }

        targets.SortBy(x => ToolsLibrary.GetDistance(x.Position, pawn.Position));
        return targets.First();
    }
}