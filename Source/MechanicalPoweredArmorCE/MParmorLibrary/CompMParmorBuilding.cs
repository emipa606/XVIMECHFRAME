using System.Collections.Generic;
using Verse;

namespace MParmorLibrary;

public class CompMParmorBuilding : ThingComp
{
    public CompProperties_MParmorBuilding Props => props as CompProperties_MParmorBuilding;

    public override IEnumerable<Gizmo> CompGetGizmosExtra()
    {
        if (!Prefs.DevMode)
        {
            yield break;
        }

        yield return new Command_Action
        {
            defaultLabel = "Debug: Charge Battery10000",
            action = delegate { (parent as MParmorBuilding)?.PowerTracker.ChargeBatteryExactly(1000000); }
        };
        yield return new Command_Action
        {
            defaultLabel = "Debug: Charge Battery1000",
            action = delegate { (parent as MParmorBuilding)?.PowerTracker.ChargeBatteryExactly(100000); }
        };
        yield return new Command_Action
        {
            defaultLabel = "Debug: Charge Battery1",
            action = delegate { (parent as MParmorBuilding)?.PowerTracker.ChargeBatteryExactly(100); }
        };
    }
}