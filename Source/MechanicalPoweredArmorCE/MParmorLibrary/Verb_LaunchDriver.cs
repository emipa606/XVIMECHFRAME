using MParmorLibrary.SkillSystem;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace MParmorLibrary;

public class Verb_LaunchDriver : Verb_Jump
{
    public override float EffectiveRange => verbProps.range;

    public override bool CanHitTargetFrom(IntVec3 root, LocalTargetInfo targ)
    {
        var num = EffectiveRange * EffectiveRange;
        var cell = targ.Cell;
        return caster.Position.DistanceToSquared(cell) <= num;
    }

    public override void DrawHighlight(LocalTargetInfo target)
    {
        if (target.IsValid && JumpUtility.ValidJumpTarget(caster, caster.Map, target.Cell))
        {
            GenDraw.DrawTargetHighlightWithLayer(target.CenterVector3, AltitudeLayer.Silhouettes);
        }

        GenDraw.DrawRadiusRing(caster.Position, EffectiveRange, Color.white);
    }

    public override void OrderForceTarget(LocalTargetInfo target)
    {
        var job = JobMaker.MakeJob(RimWorld.JobDefOf.CastJump, target);
        job.verbToUse = this;
        if (CasterPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc))
        {
            FleckMaker.Static(target.Cell, CasterPawn.Map, RimWorld.FleckDefOf.FeedbackGoto);
        }
    }

    protected override bool TryCastShot()
    {
        var casterPawn = CasterPawn;
        var cell = currentTarget.Cell;
        var map = casterPawn.Map;
        if (DirectOwner is SkillObject skillObject)
        {
            (skillObject.parent.parent as MParmorSelfDestruct)?.SelfDestruct();
        }

        var pawnFlyer = PawnFlyer.MakeFlyer(ThingDefOf.XFMParmor_PawnJumper_LaunchDriver, casterPawn, cell, null, null);
        if (pawnFlyer == null)
        {
            return false;
        }

        GenSpawn.Spawn(pawnFlyer, cell, map);
        return true;
    }
}