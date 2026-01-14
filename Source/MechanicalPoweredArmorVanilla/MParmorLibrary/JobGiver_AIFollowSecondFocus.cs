using RimWorld;
using Verse;
using Verse.AI;

namespace MParmorLibrary;

public class JobGiver_AIFollowSecondFocus : JobGiver_AIFollowPawn
{
    public float radius;

    protected override int FollowJobExpireInterval => 120;

    public override ThinkNode DeepCopy(bool resolve = true)
    {
        var jobGiver_AIFollowSecondFocus = (JobGiver_AIFollowSecondFocus)base.DeepCopy(resolve);
        jobGiver_AIFollowSecondFocus.radius = radius;
        return jobGiver_AIFollowSecondFocus;
    }

    protected override Pawn GetFollowee(Pawn pawn)
    {
        return (Pawn)pawn.mindState.duty.focusSecond.Thing;
    }

    protected override float GetRadius(Pawn pawn)
    {
        return radius;
    }
}