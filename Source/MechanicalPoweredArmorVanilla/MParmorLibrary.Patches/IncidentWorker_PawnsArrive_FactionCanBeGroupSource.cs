using HarmonyLib;
using RimWorld;

namespace MParmorLibrary.Patches;

[HarmonyPatch(typeof(IncidentWorker_PawnsArrive), nameof(IncidentWorker_PawnsArrive.FactionCanBeGroupSource))]
public static class IncidentWorker_PawnsArrive_FactionCanBeGroupSource
{
    private static void Postfix(Faction f, ref bool __result)
    {
        if (f.def.defName == "XFMParmor_AntiMParmor")
        {
            __result = false;
        }
    }
}