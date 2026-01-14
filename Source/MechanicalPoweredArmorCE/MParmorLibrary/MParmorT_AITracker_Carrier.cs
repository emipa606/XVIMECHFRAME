using MParmorLibrary.SkillSystem;
using Verse;

namespace MParmorLibrary;

public class MParmorT_AITracker_Carrier : MParmorT_AITracker
{
    private MParmorCore_Carrier Core => core as MParmorCore_Carrier;

    public override void GetHurt_Machine(DamageInfo dinfo)
    {
        GetHurt(dinfo);
    }

    public override void GetHurt_Shield(DamageInfo dinfo)
    {
        GetHurt(dinfo);
    }

    public override void ComradesHurted(Pawn pawn, DamageInfo dinfo, ref bool accepted)
    {
        if (!accepted && Core.DronesCount >= 2)
        {
            accepted = TryProtect(dinfo, pawn);
        }
    }

    private void GetHurt(DamageInfo dinfo)
    {
        if (!dinfo.Def.harmsHealth)
        {
            return;
        }

        var wearer = core.Wearer;
        TryProtect(dinfo, wearer);
    }

    private bool TryProtect(DamageInfo dinfo, Pawn target)
    {
        if (!ToolsLibrary_MParmorOnly.IsUnfriendly(dinfo, target))
        {
            return false;
        }

        if (dinfo.IsMeleeDamage(target))
        {
            var skillObject = core.GetComp<CompSkills>().FindSkill("AttactMode");
            if (skillObject != null && skillObject.CanUsed(out _))
            {
                skillObject.VerbTracker.PrimaryVerb.TryStartCastOn(dinfo.Instigator);
                return true;
            }
        }

        if (!dinfo.IsRangedDamage(target))
        {
            return false;
        }

        var skillObject2 = core.GetComp<CompSkills>().FindSkill("DefenceMode");
        if (skillObject2 == null || !skillObject2.CanUsed(out _))
        {
            return false;
        }

        skillObject2.VerbTracker.PrimaryVerb.TryStartCastOn(target);
        return true;
    }

    public override bool? TryStartAttact(LocalTargetInfo target)
    {
        if (base.TryStartAttact(target) == false)
        {
            return null;
        }

        var thing = target.Thing;
        if (thing == null || Core.DronesCount <= 2)
        {
            return null;
        }

        var skillObject = core.GetComp<CompSkills>().FindSkill("AttactMode");
        if (skillObject == null || !skillObject.CanUsed(out _))
        {
            return null;
        }

        skillObject.VerbTracker.PrimaryVerb.TryStartCastOn(thing);
        return true;
    }
}