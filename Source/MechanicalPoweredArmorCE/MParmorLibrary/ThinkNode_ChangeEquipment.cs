using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace MParmorLibrary;

public class ThinkNode_ChangeEquipment : ThinkNode
{
    public override ThinkResult TryIssueJobPackage(Pawn pawn, JobIssueParams jobParams)
    {
        if (pawn.equipment.Primary == null &&
            pawn.TryFindThingByWeaponTagFromInventory("XFMParmor_AntiMPArmor", out var foundThing))
        {
            var thingWithComps = foundThing as ThingWithComps;
            pawn.inventory.innerContainer.Remove(thingWithComps);
            pawn.equipment.AddEquipment(thingWithComps);
            return ThinkResult.NoJob;
        }

        if (!IsRocketer(pawn) || pawn.equipment.Primary?.def.defName == "XFMParmor_RocketLauncher" ||
            !pawn.TryFindThingByDefFromInventory("XFMParmor_RocketLauncher", out var foundThing2))
        {
            return ThinkResult.NoJob;
        }

        var thingWithComps2 = foundThing2 as ThingWithComps;
        pawn.equipment.TryTransferEquipmentToContainer(pawn.equipment.Primary,
            pawn.inventory.GetDirectlyHeldThings());
        pawn.inventory.innerContainer.Remove(thingWithComps2);
        pawn.equipment.AddEquipment(thingWithComps2);

        return ThinkResult.NoJob;
    }

    private static bool IsRocketer(Pawn pawn)
    {
        return pawn.GetLord()?.CurLordToil is LordToil_AntiMParmorFight lordToil_AntiMParmorFight &&
               lordToil_AntiMParmorFight.Data.rocketer == pawn;
    }
}