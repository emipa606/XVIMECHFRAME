using Verse;

namespace MParmorLibrary;

public class MParmorT_PowerTracker(IMParmorInstance instance) : IExposable
{
    private int batteryLevel;
    public IMParmorInstance instance = instance;

    public int BatteryCapacity =>
        instance.ModulesTracker.TryGetModule("Module_LargeBattery", out Module_LargeBattery module)
            ? module.Battery
            : 200000;

    public bool IsFull => batteryLevel == BatteryCapacity * 100f;

    public bool IsLow => Battery <= 10000.0;

    public bool IsEmpty => Battery <= 1f;

    public float ElectricityPercent => IsFull ? 1f : Battery / BatteryCapacity;

    public string ElectiricityLabel => $"{(int)Battery:N0} / {BatteryCapacity:N0}";

    public float Battery
    {
        get => IsFull ? BatteryCapacity : (int)(batteryLevel * 0.01f);
        set
        {
            batteryLevel = (int)((value * 100f) + 0.99999f);
            if (batteryLevel > BatteryCapacity * 100)
            {
                batteryLevel = BatteryCapacity * 100;
            }

            if (batteryLevel < 0)
            {
                batteryLevel = 0;
            }
        }
    }

    public void ExposeData()
    {
        Scribe_Values.Look(ref batteryLevel, "batteryLevel");
    }

    public void ChargeBatteryExactly(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        batteryLevel += amount;
        if (batteryLevel > BatteryCapacity * 100)
        {
            batteryLevel = BatteryCapacity * 100;
        }
    }

    public void ConsumeBatteryExactly(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        batteryLevel -= amount;
        if (batteryLevel < 0)
        {
            batteryLevel = 0;
        }
    }

    private PowerCell CreateNewPowerCell()
    {
        var powerCell = ThingMaker.MakeThing(ThingDefOf.XFMParmor_UnfilledPowerCell) as PowerCell;
        powerCell?.batteryLevel = batteryLevel % 20000000;
        return powerCell;
    }

    public void ChangeNewPowerCell()
    {
        var powerCell = CreateNewPowerCell();
        var mParmorBuilding = instance as MParmorBuilding;
        batteryLevel -= powerCell.batteryLevel;
        batteryLevel += 20000000;
        if (mParmorBuilding != null)
        {
            GenSpawn.Spawn(powerCell, mParmorBuilding.Position, mParmorBuilding.Map, WipeMode.VanishOrMoveAside);
        }
    }

    public bool Tick(Pawn driver)
    {
        if (IsEmpty)
        {
            return false;
        }

        ConsumeBatteryExactly(25);
        if (!IsLow && driver.pather.MovingNow)
        {
            ConsumeBatteryExactly(25);
        }

        return true;
    }
}