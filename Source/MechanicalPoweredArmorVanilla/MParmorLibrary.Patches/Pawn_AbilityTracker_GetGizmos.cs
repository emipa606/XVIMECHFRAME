using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace MParmorLibrary.Patches;

[HarmonyPatch(typeof(Pawn_AbilityTracker), nameof(Pawn_AbilityTracker.GetGizmos))]
public static class Pawn_AbilityTracker_GetGizmos
{
    private static void Postfix(Pawn_AbilityTracker __instance, ref IEnumerable<Gizmo> __result)
    {
        if (PatchMain.CheckPawnIsInMParmor(__instance.pawn, out _))
        {
            __result = new List<Gizmo>();
        }
    }
}