using HarmonyLib;
using MParmorLibrary.SingleObject;
using Verse;

namespace MParmorLibrary.Patches;

[HarmonyPatch(typeof(PawnUIOverlay), nameof(PawnUIOverlay.DrawPawnGUIOverlay))]
public static class PawnUIOverlay_DrawPawnGUIOverlay
{
    private static bool Prefix()
    {
        AcquisitionManagement.instanceBoolForDrawPawnGUIOverlay = true;
        return true;
    }

    private static void Postfix()
    {
        AcquisitionManagement.instanceBoolForDrawPawnGUIOverlay = false;
    }
}