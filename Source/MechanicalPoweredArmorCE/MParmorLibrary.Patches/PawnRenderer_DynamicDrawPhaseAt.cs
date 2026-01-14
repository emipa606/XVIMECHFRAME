using System.Reflection;
using HarmonyLib;
using UnityEngine;
using Verse;

namespace MParmorLibrary.Patches;

[HarmonyPatch(typeof(PawnRenderer), nameof(PawnRenderer.DynamicDrawPhaseAt))]
public static class PawnRenderer_DynamicDrawPhaseAt
{
    private static bool Prefix(PawnRenderer __instance, DrawPhase phase)
    {
        var pawn = (Pawn)typeof(PawnRenderer).GetField("pawn", BindingFlags.Instance | BindingFlags.NonPublic)
            ?.GetValue(__instance);
        var mparmorCore = pawn.GetMParmorCore(out _);
        if (!mparmorCore)
        {
            return true;
        }

        var value = typeof(PawnRenderer).GetField("results", BindingFlags.Instance | BindingFlags.NonPublic)
            ?.GetValue(__instance);
        if (phase != DrawPhase.Draw)
        {
            return true;
        }

        if (value == null)
        {
            return false;
        }

        var pawnDrawParms = (PawnDrawParms)value.GetType()
            .GetField("parms", BindingFlags.Instance | BindingFlags.Public)
            ?.GetValue(value)!;
        var vector = (Vector3)value.GetType().GetField("bodyPos", BindingFlags.Instance | BindingFlags.Public)
            ?.GetValue(value)!;
        _ = (float)value.GetType().GetField("bodyAngle", BindingFlags.Instance | BindingFlags.Public)
            ?.GetValue(value)!;
        var facing = pawnDrawParms.facing;
        var vector2 = vector.WithYOffset(PawnRenderUtility.AltitudeForLayer(facing == Rot4.North ? -10f : 90f));
        PawnRenderUtility.DrawEquipmentAndApparelExtras(pawn, vector2, facing, pawnDrawParms.flags);

        return false;
    }
}