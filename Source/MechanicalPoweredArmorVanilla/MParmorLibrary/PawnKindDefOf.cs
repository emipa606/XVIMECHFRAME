using RimWorld;
using Verse;

namespace MParmorLibrary;

[DefOf]
public static class PawnKindDefOf
{
    public static PawnKindDef XFMParmor_Drone_Carrier;

    static PawnKindDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(PawnKindDefOf));
    }
}