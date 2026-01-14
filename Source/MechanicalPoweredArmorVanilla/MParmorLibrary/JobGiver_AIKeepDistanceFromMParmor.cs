using RimWorld;
using Verse;
using Verse.AI;

namespace MParmorLibrary;

public class JobGiver_AIKeepDistanceFromMParmor : ThinkNode_JobGiver
{
    public float distance;

    public override ThinkNode DeepCopy(bool resolve = true)
    {
        var jobGiver_AIKeepDistanceFromMParmor = (JobGiver_AIKeepDistanceFromMParmor)base.DeepCopy(resolve);
        jobGiver_AIKeepDistanceFromMParmor.distance = distance;
        return jobGiver_AIKeepDistanceFromMParmor;
    }

    protected override Job TryGiveJob(Pawn pawn)
    {
        if (PawnUtility.PlayerForcedJobNowOrSoon(pawn))
        {
            return null;
        }

        IntVec3? intVec = null;
        foreach (var item in ToolsLibrary_MParmorOnly.GetMParmor(pawn.Map))
        {
            if ((item.Wearer.Position - pawn.Position).LengthHorizontalSquared < distance * distance)
            {
                intVec = item.Wearer.Position;
            }
        }

        if (!intVec.HasValue)
        {
            return null;
        }

        if (!RCellFinder.TryFindDirectFleeDestination(intVec.Value, distance, pawn, out var result))
        {
            return null;
        }

        var job = JobMaker.MakeJob(RimWorld.JobDefOf.Goto, result);
        job.locomotionUrgency = LocomotionUrgency.Sprint;
        return job;
    }
}