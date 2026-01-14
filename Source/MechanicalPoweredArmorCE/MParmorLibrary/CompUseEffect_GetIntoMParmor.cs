using MParmorLibrary.Settings;
using MParmorLibrary.SingleObject;
using RimWorld;
using Verse;

namespace MParmorLibrary;

public class CompUseEffect_GetIntoMParmor : CompUseEffect
{
    private CompMParmorBuilding comp;

    public override void PostSpawnSetup(bool respawningAfterLoad)
    {
        comp = parent.GetComp<CompMParmorBuilding>();
    }

    public override AcceptanceReport CanBeUsedBy(Pawn p)
    {
        string text = null;
        if (p.skills.GetSkill(SkillDefOf.Shooting).Level < 12)
        {
            text += "XFMParmor_CompUseEffect_GetIntoMParmor_failReasonA".Translate();
        }

        if (p.skills.GetSkill(SkillDefOf.Intellectual).Level < 8)
        {
            if (text != null)
            {
                text += ",";
            }

            text += "XFMParmor_CompUseEffect_GetIntoMParmor_failReasonB".Translate();
        }

        if (((MParmorBuilding)parent).PowerTracker.IsEmpty)
        {
            if (text != null)
            {
                text += ",";
            }

            text += "XFMParmor_CompUseEffect_GetIntoMParmor_failReasonC".Translate();
        }

        if (p.def.defName == "SoSHologramRace")
        {
            if (text != null)
            {
                text += ",";
            }

            text += "XFMParmor_CompUseEffect_GetIntoMParmor_failReasonE".Translate();
        }

        foreach (var apparel in p.apparel.WornApparel)
        {
            if (apparel is not MParmorCore)
            {
                continue;
            }

            return "XFMParmor_CompUseEffect_GetIntoMParmor_failReasonD".Translate(p.LabelCap);
        }

        if (text != null)
        {
            return text;
        }

        return true;
    }

    public override void DoEffect(Pawn usedBy)
    {
        var position = parent.Position;
        var mParmorCore = ThingMaker.MakeThing(comp.Props.mechanicalArmor.core) as MParmorCore;
        mParmorCore?.CopyTracker(parent as MParmorBuilding);
        mParmorCore?.isNaturallySpawned = false;
        usedBy.apparel.Wear(mParmorCore, true, true);
        AcquisitionManagement.GetInstance().cores.Add(mParmorCore);
        parent.Destroy();
        usedBy.Position = position;
        usedBy.Notify_Teleported();
        usedBy.drafter.Drafted = true;
        if (usedBy.equipment.Primary != null)
        {
            if (usedBy.Spawned && !Setting.Settings.putWeaponIntoInventory)
            {
                usedBy.equipment.TryDropEquipment(usedBy.equipment.Primary, out _, usedBy.Position);
            }
            else
            {
                usedBy.equipment.TryTransferEquipmentToContainer(usedBy.equipment.Primary,
                    usedBy.inventory.GetDirectlyHeldThings());
            }
        }

        usedBy.equipment.AddEquipment(ThingMaker.MakeThing(mParmorCore?.System.weapon[0]) as ThingWithComps);
        mParmorCore?.ReloadAmmoForPrimary();
    }
}