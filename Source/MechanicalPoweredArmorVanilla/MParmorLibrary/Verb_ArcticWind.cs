using MParmorLibrary.SkillSystem;
using UnityEngine;
using Verse;
using Verse.AI;

namespace MParmorLibrary;

public class Verb_ArcticWind : Verb
{
    public override void DrawHighlight(LocalTargetInfo target)
    {
        GenDraw.DrawTargetHighlightWithLayer(target.CenterVector3, AltitudeLayer.Silhouettes);
        GenDraw.DrawRadiusRing(target.Cell, 7.9f, new Color(0.30980393f, 83f / 85f, 0.9372549f, 0.75f));
        GenDraw.DrawRadiusRing(caster.Position, EffectiveRange, Color.white);
    }

    protected override bool TryCastShot()
    {
        var casterPawn = CasterPawn;
        if (casterPawn == null)
        {
            return false;
        }

        if (DirectOwner is not SkillObject skillObject)
        {
            return false;
        }

        skillObject.UsedOnce();
        if (ThingMaker.MakeThing(ThingDefOf.XFMParmor_ArcticWind) is not ArcticWind arcticWind)
        {
            return true;
        }

        arcticWind.source = casterPawn;
        GenSpawn.Spawn(arcticWind, currentTarget.Cell, CasterPawn.Map);

        return true;
    }

    public override void OrderForceTarget(LocalTargetInfo target)
    {
        var job = JobMaker.MakeJob(RimWorld.JobDefOf.UseVerbOnThing, target);
        job.verbToUse = this;
        CasterPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
    }
}