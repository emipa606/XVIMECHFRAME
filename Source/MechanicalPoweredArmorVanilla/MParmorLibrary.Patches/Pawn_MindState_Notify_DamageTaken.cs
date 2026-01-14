using System.Collections.Generic;
using HarmonyLib;
using Verse;
using Verse.AI;

namespace MParmorLibrary.Patches;

[HarmonyPatch(typeof(Pawn_MindState), nameof(Pawn_MindState.Notify_DamageTaken))]
public static class Pawn_MindState_Notify_DamageTaken
{
    private static readonly List<Pawn> pawns = [];

    private static void Postfix(DamageInfo dinfo, Pawn_MindState __instance)
    {
        var pawn = __instance.pawn;
        if (pawn.Faction == null || !pawn.Spawned)
        {
            return;
        }

        var accepted = false;
        pawns.AddRange(pawn.Map.mapPawns.SpawnedPawnsInFaction(pawn.Faction));
        foreach (var pawn2 in pawns)
        {
            if (pawn2.GetMParmorCore(out var core) && core.AiTracker.IsActive)
            {
                core.AiTracker.ComradesHurted(pawn, dinfo, ref accepted);
            }
        }

        pawns.Clear();
    }
}