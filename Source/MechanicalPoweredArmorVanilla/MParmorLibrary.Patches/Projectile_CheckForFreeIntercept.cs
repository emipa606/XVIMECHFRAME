using System.Collections.Generic;
using HarmonyLib;
using Verse;

namespace MParmorLibrary.Patches;

[HarmonyPatch(typeof(Projectile), "CheckForFreeIntercept", typeof(IntVec3))]
public static class Projectile_CheckForFreeIntercept
{
    private static bool Prefix(Projectile __instance, IntVec3 c, ref bool __result)
    {
        IEnumerable<IIntercept> enumerable = Intercepts.intercepts.TryGetValue(__instance.Map);
        foreach (var item in enumerable ?? [])
        {
            if (!item.CanIntercept(__instance, c) || !item.TryIntercept(__instance, c))
            {
                continue;
            }

            __result = true;
            return false;
        }

        var thingList = c.GetThingList(__instance.Map);
        foreach (var item2 in thingList)
        {
            if (item2 is not ShieldsBarrierNew shieldsBarrierNew ||
                !shieldsBarrierNew.CheckForFreeIntercept(__instance, c))
            {
                continue;
            }

            __result = true;
            return false;
        }

        return true;
    }
}