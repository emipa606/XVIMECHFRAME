using Verse;
using Verse.AI;

namespace MParmorLibrary;

public class JobGiver_GotoOrFollowTarget : ThinkNode_JobGiver
{
    protected override Job TryGiveJob(Pawn pawn)
    {
        if (pawn is not Drone drone || drone.forceTarget == null)
        {
            return null;
        }

        if ((drone.forceTarget.Cell - pawn.Position).LengthHorizontalSquared < 2.25f)
        {
            return null;
        }

        var pawn2 = drone.forceTarget.Pawn;
        if (pawn2 is { Spawned: true })
        {
            var job = JobMaker.MakeJob(RimWorld.JobDefOf.FollowClose, pawn2);
            job.expiryInterval = 240;
            job.checkOverrideOnExpire = true;
            job.followRadius = 1.5f;
            return job;
        }

        var job2 = JobMaker.MakeJob(RimWorld.JobDefOf.Goto, drone.forceTarget.Cell);
        job2.locomotionUrgency = LocomotionUrgency.Sprint;
        return job2;
    }
}