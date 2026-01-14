using CombatExtended;
using HarmonyLib;

namespace MParmorLibrary.Patches;

[HarmonyPatch(typeof(ExplosionCE), nameof(ExplosionCE.ExplosionCellsToHit), MethodType.Getter)]
public static class ExplosionCE_ExplosionCellsToHit
{
    private static bool Prefix(ExplosionCE __instance)
    {
        if (DamageTypes.CrossShieldDamages.Contains(__instance.damType))
        {
            return true;
        }

        ThingDefOf.XFMParmor_Black_Shield.fillPercent = 1f;
        return true;
    }

    private static void Postfix()
    {
        ThingDefOf.XFMParmor_Black_Shield.fillPercent = 0f;
    }
}