using HarmonyLib;
using RimWorld;

namespace MParmorLibrary.Patches;

[HarmonyPatch(typeof(Building_WorkTable), nameof(Building_WorkTable.UsableForBillsAfterFueling))]
public static class Building_WorkTable_UsableForBillsAfterFueling
{
    private static void Postfix(Building_WorkTable __instance, ref bool __result)
    {
        if (__instance is Building_FabricationPit building_FabricationPit &&
            (building_FabricationPit.CentralSystem == null || building_FabricationPit.State != FabricationState.Free))
        {
            __result = false;
        }
    }
}