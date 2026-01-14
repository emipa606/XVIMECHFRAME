using Verse;
using Verse.AI.Group;

namespace MParmorLibrary;

public class Trigger_NonMParmor : Trigger
{
    public override bool ActivateOn(Lord lord, TriggerSignal signal)
    {
        if (MParmorBuilding.Cache.Any(x => x.Map == lord.Map))
        {
            return false;
        }

        return !ToolsLibrary_MParmorOnly.GetMParmor(lord.Map).Any();
    }
}