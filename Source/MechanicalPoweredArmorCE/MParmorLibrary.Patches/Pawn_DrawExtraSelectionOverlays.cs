using HarmonyLib;
using UnityEngine;
using Verse;

namespace MParmorLibrary.Patches;

[HarmonyPatch(typeof(Pawn), nameof(Pawn.DrawExtraSelectionOverlays), [])]
public static class Pawn_DrawExtraSelectionOverlays
{
    private static void Postfix(Pawn __instance)
    {
        if (__instance.GetMParmorSelfDestruct())
        {
            GenDraw.DrawRadiusRing(__instance.Position, MParmorWreckage.radius,
                new Color(0.30980393f, 83f / 85f, 0.9372549f, 0.75f),
                c => GenSight.LineOfSight(__instance.Position, c, __instance.Map));
        }
    }
}