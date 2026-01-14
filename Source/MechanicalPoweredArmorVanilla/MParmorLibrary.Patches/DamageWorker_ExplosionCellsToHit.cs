using HarmonyLib;
using Verse;

namespace MParmorLibrary.Patches;

[HarmonyPatch(typeof(DamageWorker), nameof(DamageWorker.ExplosionCellsToHit), typeof(IntVec3), typeof(Map),
    typeof(float),
    typeof(IntVec3?), typeof(IntVec3?), typeof(FloatRange?))]
public static class DamageWorker_ExplosionCellsToHit
{
    private static void Prefix(DamageWorker __instance)
    {
        if (DamageTypes.CrossShieldDamages.Contains(__instance.def))
        {
            return;
        }

        ThingDefOf.XFMParmor_Black_Shield.fillPercent = 1f;
    }

    private static void Postfix()
    {
        ThingDefOf.XFMParmor_Black_Shield.fillPercent = 0f;
    }
}