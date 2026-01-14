using MParmorLibrary.SkillSystem;
using Verse;
using Verse.Sound;

namespace MParmorLibrary;

public class Verb_AquaChangeMode : Verb
{
    protected override bool TryCastShot()
    {
        if (DirectOwner is SkillObject skillObject)
        {
            skillObject.UsedOnce();
        }

        if (CasterPawn.equipment.Primary == null)
        {
            CasterPawn.equipment.AddEquipment(ThingMaker.MakeThing(ThingDefOf.XFMParmor_Weapon_Aqua) as ThingWithComps);
            SoundDefOf.Interact_ChargeRifle.PlayOneShot(new TargetInfo(CasterPawn.Position, CasterPawn.Map));
            return true;
        }

        if (CasterPawn.equipment.Primary.def.defName == "XFMParmor_Weapon_Aqua")
        {
            CasterPawn.equipment.Remove(CasterPawn.equipment.Primary);
            CasterPawn.equipment.AddEquipment(
                ThingMaker.MakeThing(ThingDefOf.XFMParmor_Weapon_AquaB) as ThingWithComps);
            SoundDefOf.Interact_ChargeRifle.PlayOneShot(new TargetInfo(CasterPawn.Position, CasterPawn.Map));
            return true;
        }

        CasterPawn.equipment.Remove(CasterPawn.equipment.Primary);
        CasterPawn.equipment.AddEquipment(ThingMaker.MakeThing(ThingDefOf.XFMParmor_Weapon_Aqua) as ThingWithComps);
        SoundDefOf.Interact_ChargeRifle.PlayOneShot(new TargetInfo(CasterPawn.Position, CasterPawn.Map));
        return true;
    }

    public void UseOnce()
    {
        TryCastShot();
    }
}