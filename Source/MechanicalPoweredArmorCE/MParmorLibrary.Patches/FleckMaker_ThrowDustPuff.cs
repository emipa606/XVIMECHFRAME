using HarmonyLib;
using RimWorld;
using Verse;

namespace MParmorLibrary.Patches;

[HarmonyPatch(typeof(FleckMaker), nameof(FleckMaker.ThrowDustPuff), typeof(IntVec3), typeof(Map), typeof(float))]
public static class FleckMaker_ThrowDustPuff
{
    public static bool throwPuff = true;

    private static bool Prefix()
    {
        return throwPuff;
    }
}