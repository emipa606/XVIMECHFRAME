using HarmonyLib;
using Verse;

namespace MParmorLibrary.Patches;

[HarmonyPatch(typeof(VerbProperties), nameof(VerbProperties.GetForceMissFactorFor))]
public static class VerbProperties_GetForceMissFactorFor
{
    private static bool Prefix(Thing equipment, ref float __result)
    {
        if (equipment != null)
        {
            return true;
        }

        __result = 1f;
        return false;
    }
}