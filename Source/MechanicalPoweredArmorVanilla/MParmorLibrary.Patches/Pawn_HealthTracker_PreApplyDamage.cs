using HarmonyLib;
using Verse;

namespace MParmorLibrary.Patches;

[HarmonyPatch(typeof(Pawn_HealthTracker), nameof(Pawn_HealthTracker.PreApplyDamage))]
public static class Pawn_HealthTracker_PreApplyDamage
{
    private static bool Prefix(DamageInfo dinfo, out bool absorbed, Pawn ___pawn)
    {
        absorbed = false;
        if (!___pawn.GetMParmorCore(out var core))
        {
            return true;
        }

        absorbed = true;
        core.AbsorbDamage(dinfo);
        return false;
    }
}