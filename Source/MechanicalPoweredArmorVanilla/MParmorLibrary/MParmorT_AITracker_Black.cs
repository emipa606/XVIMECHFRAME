using MParmorLibrary.SkillSystem;
using RimWorld;
using Verse;

namespace MParmorLibrary;

public class MParmorT_AITracker_Black : MParmorT_AITracker
{
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
        if (!dinfo.Def.harmsHealth)
        {
            return;
        }

        if (dinfo.IsRangedDamage(core.Wearer))
        {
            var skillObject = core.GetComp<CompSkills>().FindSkill("ShieldBarrier");
            if (skillObject != null && skillObject.CanUsed(out _))
            {
                skillObject.VerbTracker.PrimaryVerb.TryStartCastOn(core.Wearer.Position +
                                                                   ToolsLibrary
                                                                       .RotFromPosition(core.Wearer.TrueCenter(),
                                                                           dinfo.Instigator.TrueCenter()).FacingCell);
            }
        }

        if (!dinfo.IsMeleeDamage(core.Wearer))
        {
            return;
        }

        var skillObject2 = core.GetComp<CompSkills>().FindSkill("ShockWave");
        if (skillObject2 != null && skillObject2.CanUsed(out _))
        {
            (skillObject2.VerbTracker.PrimaryVerb as Verb_ShockWave)?.StartSkill();
        }
    }

    public override bool? TryStartAttact(LocalTargetInfo target)
    {
        if (base.TryStartAttact(target) == false)
        {
            return null;
        }

        var thing = target.Thing;
        if (thing == null)
        {
            return null;
        }

        var skillObject = core.GetComp<CompSkills>().FindSkill("Rockets");
        if (skillObject == null || !skillObject.CanUsed(out _))
        {
            return null;
        }

        if (thing is Pawn pawn && pawn.pather.MovingNow)
        {
            skillObject.VerbTracker.PrimaryVerb.TryStartCastOn(thing);
            return true;
        }

        skillObject.VerbTracker.PrimaryVerb.TryStartCastOn(thing.Position);
        return true;
    }
}