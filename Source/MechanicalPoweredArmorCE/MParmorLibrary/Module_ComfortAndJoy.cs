using RimWorld;
using Verse;
using Verse.AI;

namespace MParmorLibrary;

public class Module_ComfortAndJoy : Module
{
    public override void TickCore()
    {
        var wearer = Core.Wearer;
        wearer.needs?.comfort?.ComfortUsed(1f);
        if (wearer.CurJob.GetCachedDriver(wearer) is JobDriver_Wait && !wearer.stances.FullBodyBusy)
        {
            Core.Wearer.needs?.joy?.GainJoy(0.000115200004f, DefDatabase<JoyKindDef>.GetNamedSilentFail("Television"));
        }
    }
}