using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace MParmorLibrary.Patches;

[HarmonyPatch(typeof(Pawn_ApparelTracker), nameof(Pawn_ApparelTracker.GetGizmos))]
public static class Pawn_ApparelTracker_GetGizmos
{
    private static void Postfix(Pawn_ApparelTracker __instance, ref IEnumerable<Gizmo> __result)
    {
        if (PatchMain.CheckPawnIsInMParmor(__instance.pawn, out var core))
        {
            __result = core.GetWornGizmos();
        }
    }
}