using System.Collections.Generic;
using CombatExtended;
using HarmonyLib;
using Verse;

namespace MParmorLibrary.Patches;

[HarmonyPatch(typeof(ProjectileCE), "CheckCellForCollision", typeof(IntVec3))]
public static class CheckCellForCollision_ProjectileCE
{
    private static bool Prefix(ProjectileCE __instance, IntVec3 cell, ref bool __result)
    {
        IEnumerable<IIntercept> enumerable = Intercepts.intercepts.TryGetValue(__instance.Map);
        foreach (var item in enumerable ?? [])
        {
            if (!item.CanIntercept(__instance, cell) || !item.TryIntercept(__instance, cell))
            {
                continue;
            }

            __result = true;
            return false;
        }

        var thingList = cell.GetThingList(__instance.Map);
        foreach (var item2 in thingList)
        {
            if (item2 is not ShieldsBarrierNew shieldsBarrierNew ||
                !shieldsBarrierNew.CheckForFreeIntercept(__instance))
            {
                continue;
            }

            __result = true;
            return false;
        }

        return true;
    }
}