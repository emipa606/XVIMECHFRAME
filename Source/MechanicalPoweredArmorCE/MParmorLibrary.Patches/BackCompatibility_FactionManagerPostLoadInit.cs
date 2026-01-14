using HarmonyLib;
using Verse;

namespace MParmorLibrary.Patches;

[HarmonyPatch(typeof(BackCompatibility), nameof(BackCompatibility.FactionManagerPostLoadInit))]
public static class BackCompatibility_FactionManagerPostLoadInit
{
    private static bool Prefix()
    {
        FixNewFaction.FixNewFaction_AntiMParmor();
        return true;
    }
}