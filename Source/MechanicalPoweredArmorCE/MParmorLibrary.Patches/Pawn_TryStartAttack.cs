using HarmonyLib;
using Verse;

namespace MParmorLibrary.Patches;

[HarmonyPatch(typeof(Pawn), nameof(Pawn.TryStartAttack))]
public static class Pawn_TryStartAttack
{
    private static bool Prefix(LocalTargetInfo targ, Pawn __instance, ref bool __result)
    {
        if (!__instance.GetMParmorCore(out var core) || !core.AiTracker.IsActive)
        {
            return true;
        }

        var startAttact = core.AiTracker.TryStartAttact(targ);
        if (!startAttact.HasValue)
        {
            return true;
        }

        __result = startAttact.Value;
        return false;
    }
}