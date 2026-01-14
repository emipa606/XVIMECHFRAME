using System;
using Verse;
using Verse.AI;

namespace MParmorLibrary;

public class ThinkNode_DroneState : ThinkNode
{
    public override ThinkResult TryIssueJobPackage(Pawn pawn, JobIssueParams jobParams)
    {
        if (pawn is not Drone drone)
        {
            return ThinkResult.NoJob;
        }

        var result = ThinkResult.NoJob;
        try
        {
            result = !drone.isAttactMode
                ? subNodes[0].TryIssueJobPackage(pawn, jobParams)
                : subNodes[1].TryIssueJobPackage(pawn, jobParams);
        }
        catch (Exception ex)
        {
            Log.Error($"Exception in {GetType()} TryIssueJobPackage: {ex}");
        }

        return result.IsValid ? result : ThinkResult.NoJob;
    }
}