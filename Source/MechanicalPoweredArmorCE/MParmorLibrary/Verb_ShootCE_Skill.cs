using CombatExtended;
using MParmorLibrary.SkillSystem;
using RimWorld;
using Verse;
using Verse.AI;

namespace MParmorLibrary;

public class Verb_ShootCE_Skill : Verb_ShootCE
{
    public MechanicalArmorDef MPArmor =>
        (DirectOwner as SkillObject)?.parent.parent.GetComp<CompMechanicalArmor>().MPArmor;

    public override bool MultiSelect => true;

    public override bool TryCastShot()
    {
        if (CasterPawn == null)
        {
            return false;
        }

        var skillObject = DirectOwner as SkillObject;
        skillObject?.UsedOnce();
        return TryCastShot();
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
            OnGUI(target);
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

    protected override bool OnCastSuccessful()
    {
        return true;
    }
}