using MParmorLibrary.SkillSystem;
using Verse;
using Verse.AI;

namespace MParmorLibrary;

public class Verb_DispatchDrone_Defend : Verb, IVerbSkillProperties
{
    public MParmorCore_Carrier Core => (DirectOwner as SkillObject)?.parent.parent as MParmorCore_Carrier;

    public bool CanUseNow(out string reason)
    {
        if (Core.DronesCount < 1)
        {
            reason = "XFMParmor_Verb_DispatchDrone_failReason".Translate();
            return false;
        }

        reason = null;
        return true;
    }

    public override bool CanHitTarget(LocalTargetInfo targ)
    {
        return CasterPawn.CanReach(targ, PathEndMode.Touch, Danger.Deadly);
    }

    public override void DrawHighlight(LocalTargetInfo target)
    {
        if (!target.IsValid)
        {
            return;
        }

        GenDraw.DrawTargetHighlight(target);
        DrawHighlightFieldRadiusAroundTarget(target);
    }

    protected override bool TryCastShot()
    {
        Drone.SpawnDroneToDefendTarget(Core, currentTarget);
        Core.SpendDroune();
        return true;
    }

    public override void OrderForceTarget(LocalTargetInfo target)
    {
        var job = JobMaker.MakeJob(RimWorld.JobDefOf.UseVerbOnThingStatic, target);
        job.verbToUse = this;
        CasterPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
    }
}