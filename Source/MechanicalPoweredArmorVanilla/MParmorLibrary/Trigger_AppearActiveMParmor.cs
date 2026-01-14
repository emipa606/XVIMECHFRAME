using Verse;
using Verse.AI.Group;

namespace MParmorLibrary;

public class Trigger_AppearActiveMParmor : Trigger
{
    public override bool ActivateOn(Lord lord, TriggerSignal signal)
    {
        return ToolsLibrary_MParmorOnly.GetMParmor(lord.Map).Any();
    }
}