using RimWorld;
using Verse;
using Verse.AI;

namespace MParmorLibrary;

public class JobGiver_AIDestructMParmor : ThinkNode_JobGiver
{
    public override ThinkNode DeepCopy(bool resolve = true)
    {
        return (JobGiver_AIDestructMParmor)base.DeepCopy(resolve);
    }

    protected override Job TryGiveJob(Pawn pawn)
    {
        if (pawn.mindState.duty == null || !pawn.mindState.duty.focus.IsValid)
        {
            return null;
        }

        var focus = pawn.mindState.duty.focus;
        if (focus.ThingDestroyed || !pawn.HostileTo(focus.Thing) ||
            !pawn.CanReach(focus, PathEndMode.Touch, Danger.Deadly))
        {
            return null;
        }

        var job = JobMaker.MakeJob(JobDefOf.XFMPamor_DestructMParmor, focus.Thing);
        job.expiryInterval = new IntRange(450, 500).RandomInRange;
        job.checkOverrideOnExpire = true;
        job.expireRequiresEnemiesNearby = true;
        return job;
    }
}