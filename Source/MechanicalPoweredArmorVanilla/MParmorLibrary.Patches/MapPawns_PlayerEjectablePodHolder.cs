using HarmonyLib;
using Verse;

namespace MParmorLibrary.Patches;

[HarmonyPatch(typeof(MapPawns), "PlayerEjectablePodHolder")]
public static class MapPawns_PlayerEjectablePodHolder
{
    private static bool Prefix(ref IThingHolder __result, Thing thing)
    {
        if (thing is not PawnSweeper pawnSweeper)
        {
            return true;
        }

        __result = pawnSweeper;
        return false;
    }
}