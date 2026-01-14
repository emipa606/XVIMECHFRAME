using System;
using Verse;

namespace MParmorLibrary;

public class MParmorT_AITracker
{
    protected MParmorCore core;
    protected bool isActive;

    public bool IsActive => isActive;

    public static MParmorT_AITracker CreateNewTracker(MParmorCore core)
    {
        var mParmorT_AITracker = (MParmorT_AITracker)Activator.CreateInstance(
            core.def.GetCompProperties<CompProperties_MechanicalArmor>().AiClass ?? typeof(MParmorT_AITracker));
        mParmorT_AITracker.core = core;
        return mParmorT_AITracker;
    }

    public Gizmo GetSwitchGizmo()
    {
        if (GetType() != typeof(MParmorT_AITracker))
        {
            return new Command_Toggle
            {
                isActive = () => isActive,
                defaultLabel = "XFMParmor_AiFight".Translate(),
                defaultDesc = core.def.GetCompProperties<CompProperties_MechanicalArmor>().AiText,
                icon = Texture2DOf.AiFight,
                activateIfAmbiguous = false,
                toggleAction = delegate { isActive = !isActive; }
            };
        }

        return null;
    }

    public virtual void Tick()
    {
    }

    public virtual void GetHurt_Machine(DamageInfo dinfo)
    {
    }

    public virtual void GetHurt_Shield(DamageInfo dinfo)
    {
    }

    public virtual void ComradesHurted(Pawn pawn, DamageInfo dinfo, ref bool accepted)
    {
    }

    public virtual bool? TryStartAttact(LocalTargetInfo target)
    {
        if (core.Wearer.stances.FullBodyBusy)
        {
            return false;
        }

        if (core.Wearer.WorkTagIsDisabled(WorkTags.Violent))
        {
            return false;
        }

        return null;
    }

    public virtual void PostExposeData()
    {
        Scribe_Values.Look(ref isActive, "isActive");
    }
}