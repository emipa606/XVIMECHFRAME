using MParmorLibrary.SkillSystem;
using RimWorld;
using Verse;
using Verse.AI;

namespace MParmorLibrary;

public class Verb_Shoot_Skill : Verb_Shoot
{
    public MechanicalArmorDef MPArmor =>
        (DirectOwner as SkillObject)?.parent.parent.GetComp<CompMechanicalArmor>().MPArmor;

    public override bool MultiSelect => true;

    protected override bool TryCastShot()
    {
        if (CasterPawn == null)
        {
            return false;
        }

        var skillObject = DirectOwner as SkillObject;
        skillObject?.UsedOnce();
        return base.TryCastShot();
    }

    public override void OrderForceTarget(LocalTargetInfo target)
    {
        var job = JobMaker.MakeJob(RimWorld.JobDefOf.UseVerbOnThingStatic, target);
        job.verbToUse = this;
        CasterPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
    }

    public override void OnGUI(LocalTargetInfo target)
    {
        if (CanHitTarget(target) && verbProps.targetParams.CanTarget(target.ToTargetInfo(caster.Map)))
        {
            base.OnGUI(target);
        }
        else
        {
            GenUI.DrawMouseAttachment(TexCommand.CannotShoot);
        }
    }

    public override bool Available()
    {
        return true;
    }
}