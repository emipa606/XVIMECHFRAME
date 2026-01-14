using Verse;
using Verse.AI.Group;

namespace MParmorLibrary;

public class Trigger_NonActiveMParmor : Trigger
{
    public override bool ActivateOn(Lord lord, TriggerSignal signal)
    {
        return !ToolsLibrary_MParmorOnly.GetMParmor(lord.Map).Any();
    }
}