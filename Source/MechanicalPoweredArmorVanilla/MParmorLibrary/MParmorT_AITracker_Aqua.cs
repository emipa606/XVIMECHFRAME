using MParmorLibrary.SkillSystem;
using Verse;
using Verse.AI;

namespace MParmorLibrary;

public class MParmorT_AITracker_Aqua : MParmorT_AITracker
{
    public bool IsLong => core.Wearer.equipment.Primary.def == ThingDefOf.XFMParmor_Weapon_AquaB;

    public override void Tick()
    {
        if (IsLong || Find.TickManager.TicksGame % 15 != 0)
        {
            return;
        }

        var skillObject = core.GetComp<CompSkills>().FindSkill("ChangeMode");
        if (skillObject == null || !skillObject.CanUsed(out _) || core.Wearer.stances.FullBodyBusy ||
            core.Wearer.CurJobDef != RimWorld.JobDefOf.Wait_Combat)
        {
            return;
        }

        if (AttackTargetFinder.BestAttackTarget(core.Wearer,
                TargetScanFlags.NeedLOSToAll | TargetScanFlags.NeedThreat | TargetScanFlags.NeedAutoTargetable, null,
                ThingDefOf.XFMParmor_Weapon_Aqua.Verbs[0].range,
                ThingDefOf.XFMParmor_Weapon_AquaB.Verbs[0].range) != null)
        {
            (skillObject.VerbTracker.PrimaryVerb as Verb_AquaChangeMode)?.UseOnce();
        }
    }

    public override void GetHurt_Machine(DamageInfo dinfo)
    {
        GetHurt(dinfo);
    }

    public override void GetHurt_Shield(DamageInfo dinfo)
    {
        GetHurt(dinfo);
    }

    private void GetHurt(DamageInfo dinfo)
    {
        if (!dinfo.Def.harmsHealth || !dinfo.IsMeleeDamage(core.Wearer))
        {
            return;
        }

        var skillObject = core.GetComp<CompSkills>().FindSkill("ColdWave");
        if (skillObject != null && skillObject.CanUsed(out _))
        {
            skillObject.VerbTracker.PrimaryVerb.TryStartCastOn(dinfo.Instigator);
        }
    }

    public override bool? TryStartAttact(LocalTargetInfo target)
    {
        if (base.TryStartAttact(target) == false)
        {
            return null;
        }

        var skillObject = core.GetComp<CompSkills>().FindSkill("ArcticWind");
        if (skillObject != null && skillObject.CanUsed(out _) && target.Thing is Pawn pawn &&
            pawn.health.hediffSet.BleedRateTotal > 0.2f)
        {
            var primaryVerb = skillObject.VerbTracker.PrimaryVerb;
            if (primaryVerb.CanHitTarget(target))
            {
                primaryVerb.TryStartCastOn(target);
                return true;
            }
        }

        var skillObject2 = core.GetComp<CompSkills>().FindSkill("ColdWave");
        if (skillObject2 != null && skillObject2.CanUsed(out _))
        {
            var primaryVerb2 = skillObject2.VerbTracker.PrimaryVerb;
            if (primaryVerb2.CanHitTarget(target))
            {
                primaryVerb2.TryStartCastOn(target);
                return true;
            }
        }

        if (!IsLong)
        {
            return null;
        }

        var num = ThingDefOf.XFMParmor_Weapon_Aqua.Verbs[0].range * 0.75f;
        if (!((target.Cell - core.Wearer.PositionHeld).LengthHorizontalSquared < num * num))
        {
            return null;
        }

        var skillObject3 = core.GetComp<CompSkills>().FindSkill("ChangeMode");
        if (skillObject3 == null || !skillObject3.CanUsed(out _))
        {
            return null;
        }

        (skillObject3.VerbTracker.PrimaryVerb as Verb_AquaChangeMode)?.UseOnce();
        core.Wearer.TryStartAttack(target);
        return true;
    }
}