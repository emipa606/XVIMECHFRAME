using Verse;
using Verse.AI.Group;

namespace MParmorLibrary;

public class LordToilData_DestructMParmor : LordToilData
{
    public Pawn destructer;

    public Thing target;

    public override void ExposeData()
    {
        Scribe_References.Look(ref destructer, "destructer");
        Scribe_References.Look(ref target, "target");
    }
}