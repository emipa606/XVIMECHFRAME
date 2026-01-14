using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace MParmorLibrary;

public class ThinkNode_CheckTarget : ThinkNode
{
    public override ThinkResult TryIssueJobPackage(Pawn pawn, JobIssueParams jobParams)
    {
        var thing = pawn.mindState.duty.focus.Thing;
        if (thing.Spawned)
        {
            return ThinkResult.NoJob;
        }

        if (thing.ParentHolder is Thing { Spawned: not false } thing2)
        {
            pawn.mindState.duty.focus = thing2;
            return ThinkResult.NoJob;
        }

        pawn.mindState.duty.focus = (pawn.GetLord().CurLordToil as LordToil_AntiMParmorFight)?.Data.driver;

        return ThinkResult.NoJob;
    }
}