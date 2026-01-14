using HarmonyLib;
using RimWorld;
using Verse;

namespace MParmorLibrary.Patches;

[HarmonyPatch(typeof(StaggerHandler), nameof(StaggerHandler.StaggerFor))]
public static class StaggerHandler_StaggerFor
{
    private static bool Prefix(Pawn ___parent)
    {
        return !___parent.GetMParmorCore();
    }
}