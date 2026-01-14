using MParmorLibrary.SkillSystem;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace MParmorLibrary;

public class Verb_JumpSkill : Verb_Jump
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
            GenDraw.DrawRadiusRing(target.Cell, PawnFlyer_RedSkill.radius,
                new Color(0.30980393f, 83f / 85f, 0.9372549f, 0.75f));
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
        var skillObject = DirectOwner as SkillObject;
        skillObject?.UsedOnce();
        var cell = currentTarget.Cell;
        var map = casterPawn.Map;
        var pawnFlyer = PawnFlyer.MakeFlyer(ThingDefOf.XFMParmor_PawnJumper_RedSkill, casterPawn, cell, null, null);
        if (pawnFlyer == null)
        {
            return false;
        }

        GenSpawn.Spawn(pawnFlyer, cell, map);
        return true;
    }
}