using MParmorLibrary.Settings;
using RimWorld;
using Verse;

namespace MParmorLibrary;

public class CompMechanicalArmor : ThingComp
{
    public MechanicalArmorDef MPArmor => Props.mechanicalArmor;

    public bool IsCore => parent.def == MPArmor.core;

    public bool IsWeapon => parent is not Apparel;

    public int TestforNumber => parent.thingIDNumber % 30;

    public CompProperties_MechanicalArmor Props => props as CompProperties_MechanicalArmor;

    public Pawn Wearer => IsWeapon ? null : (parent as Apparel)?.Wearer;

    public override void PostSpawnSetup(bool respawningAfterLoad)
    {
        base.PostSpawnSetup(respawningAfterLoad);
        Testfor();
    }

    public override void CompTick()
    {
        base.CompTick();
        if (Find.TickManager.TicksGame % 30 == TestforNumber)
        {
            Testfor();
        }
    }

    private void Testfor()
    {
        if (Wearer == null)
        {
            if (IsCore && parent.Spawned)
            {
                ChangeToBuilding();
            }
            else
            {
                parent.Destroy();
            }
        }
        else
        {
            CheckWeaponAndAddWeapons();
        }
    }

    private void CheckWeaponAndAddWeapons()
    {
        if (Wearer.equipment.Primary != null && MPArmor.weapon.Contains(Wearer.equipment.Primary.def))
        {
            return;
        }

        if (Wearer.equipment.Primary != null)
        {
            if (Wearer.Spawned && !Setting.Settings.putWeaponIntoInventory)
            {
                Wearer.equipment.TryDropEquipment(Wearer.equipment.Primary, out _, Wearer.Position);
            }
            else
            {
                Wearer.equipment.TryTransferEquipmentToContainer(Wearer.equipment.Primary,
                    Wearer.inventory.GetDirectlyHeldThings());
            }
        }

        Wearer.equipment.AddEquipment(ThingMaker.MakeThing(MPArmor.weapon[0]) as ThingWithComps);
    }

    private void ChangeToBuilding()
    {
        var mParmorBuilding = ThingMaker.MakeThing(MPArmor.building) as MParmorBuilding;
        mParmorBuilding?.CopyTracker(parent as MParmorCore);
        mParmorBuilding?.SetFactionDirect(Faction.OfPlayer);
        var position = parent.Position;
        var map = parent.Map;
        parent.Destroy();
        GenSpawn.Spawn(mParmorBuilding, position, map, WipeMode.VanishOrMoveAside);
    }
}