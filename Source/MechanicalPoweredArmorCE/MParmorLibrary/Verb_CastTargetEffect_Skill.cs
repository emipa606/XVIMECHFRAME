using MParmorLibrary.SkillSystem;
using RimWorld;
using Verse.AI;

namespace MParmorLibrary;

public class Verb_CastTargetEffect_Skill : Verb_CastBase
{
    public MechanicalArmorDef MPArmor => EquipmentSource.GetComp<CompMechanicalArmor>().MPArmor;

    public bool HaveCore
    {
        get
        {
            foreach (var apparel in CasterPawn.apparel.WornApparel)
            {
                if (apparel.def == MPArmor.core)
                {
                    return true;
                }
            }

            return false;
        }
    }

    protected override bool TryCastShot()
    {
        var casterPawn = CasterPawn;
        var thing = currentTarget.Thing;
        if (casterPawn == null)
        {
            return false;
        }

        if (!HaveCore)
        {
            casterPawn.equipment.Remove(EquipmentSource);
            return false;
        }

        foreach (var comp in EquipmentSource.GetComps<CompTargetEffect>())
        {
            comp.DoEffectOn(casterPawn, thing);
        }

        var skillObject = DirectOwner as SkillObject;
        skillObject?.UsedOnce();
        return true;
    }
}