using Verse;

namespace MParmorLibrary;

public class Stance_Parry : Stance_Busy
{
    public Stance_Parry(int ticks, LocalTargetInfo focusTarg)
    {
        ticksLeft = ticks;
        this.focusTarg = focusTarg;
    }
}