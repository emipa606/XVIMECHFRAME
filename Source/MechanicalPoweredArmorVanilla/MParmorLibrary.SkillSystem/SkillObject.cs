using System.Collections.Generic;
using RimWorld;
using Verse;

namespace MParmorLibrary.SkillSystem;

public class SkillObject : IExposable, IVerbOwner, ILoadReferenceable
{
    public int cantChargeTime;

    public int energy;
    public int num = -1;

    public CompSkills parent;

    public Skill skill;

    public Thing thingHolder;

    public VerbTracker verbTracker;

    public MParmorCore Core => parent.parent as MParmorCore;

    public MechanicalArmorDef MParmor => parent.MParmor;

    public float ChargePercent
    {
        get
        {
            if (skill.noNeedCharge)
            {
                return 1f;
            }

            return energy / (float)skill.energy;
        }
    }

    public bool IsFinishedCharge => skill.noNeedCharge || energy >= skill.energy;

    public bool CanCharging => parent.Core is { CanChargeSkills: true } && !IsFinishedCharge && skill.chargeByTime;

    public bool NeedChagre => !skill.noNeedCharge;

    public string Id => skill.id;

    public void ExposeData()
    {
        Scribe_Values.Look(ref energy, "energy");
        Scribe_Values.Look(ref cantChargeTime, "cantChargeTime");
        Scribe_References.Look(ref thingHolder, "thingHolder");
    }

    public string GetUniqueLoadID()
    {
        return $"SkillObject_{parent.parent.ThingID}_{num}";
    }

    public VerbTracker VerbTracker => verbTracker ?? (verbTracker = new VerbTracker(this));

    public List<VerbProperties> VerbProperties => [skill.verb];

    public List<Tool> Tools => null;

    public ImplementOwnerTypeDef ImplementOwnerTypeDef => ImplementOwnerTypeDefOf.NativeVerb;

    public Thing ConstantCaster => parent.Wearer;

    public string UniqueVerbOwnerID()
    {
        return $"MParmorSkillSystem_{parent.parent.ThingID}";
    }

    public bool VerbsStillUsableBy(Pawn p)
    {
        return parent.Wearer == p;
    }

    public bool CanUsed(out string reason)
    {
        reason = "";
        if (!IsFinishedCharge)
        {
            reason = "XFMParmor_SkillDisabled".Translate();
        }

        if (VerbTracker.AllVerbs[0] is not IVerbSkillProperties verbSkillProperties ||
            verbSkillProperties.CanUseNow(out var reason2))
        {
            return reason == "";
        }

        if (reason != "")
        {
            reason += ",";
        }

        reason += reason2;

        return reason == "";
    }

    public void ResetCaster()
    {
        VerbTracker.AllVerbs[0].caster = parent.Wearer;
    }

    public void Tick()
    {
        ResetCaster();
        VerbTracker.VerbsTick();
        if (cantChargeTime > 0)
        {
            cantChargeTime--;
        }
        else if (CanCharging)
        {
            energy++;
        }
    }

    public void ChargeEnergy(int amount)
    {
        if (IsFinishedCharge)
        {
            return;
        }

        energy += amount;
        if (energy > skill.energy)
        {
            energy = skill.energy;
        }
    }

    public void UsedOnce()
    {
        if (!CanUsed(out _))
        {
            return;
        }

        energy = 0;
    }

    public IEnumerable<Gizmo> GetGizmoPetSkill()
    {
        ResetCaster();
        yield return SetCommand(VerbTracker.AllVerbs[0], this);
        if (Prefs.DevMode)
        {
            yield return new Command_Action
            {
                defaultLabel = $"FinshiChage:{VerbProperties[0].label}",
                action = delegate { energy = skill.energy; }
            };
        }
    }

    public Command_SkillTarget SetCommand(Verb verb, SkillObject skill)
    {
        verb.caster = parent.Wearer;

        var command_SkillTarget = new Command_SkillTarget(this)
        {
            defaultDesc = skill.skill.description,
            defaultLabel = verb.verbProps.label,
            verb = verb,
            icon = verb.UIIcon != BaseContent.BadTex ? verb.UIIcon : BaseContent.BadTex
        };
        if (!skill.CanUsed(out var reason))
        {
            command_SkillTarget.Disable(reason);
        }

        return command_SkillTarget;
    }
}