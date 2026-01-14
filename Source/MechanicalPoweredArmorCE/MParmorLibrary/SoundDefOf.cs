using RimWorld;
using Verse;

namespace MParmorLibrary;

[DefOf]
public static class SoundDefOf
{
    public static SoundDef XFMParmor_HitMachine;

    public static SoundDef XFMParmor_DestroyMParmor;

    public static SoundDef Interact_ChargeRifle;

    static SoundDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(SoundDefOf));
    }
}