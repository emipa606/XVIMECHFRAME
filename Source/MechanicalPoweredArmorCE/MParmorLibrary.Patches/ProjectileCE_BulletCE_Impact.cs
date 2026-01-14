using System.Collections.Generic;
using CombatExtended;
using HarmonyLib;
using RimWorld;
using Verse;

namespace MParmorLibrary.Patches;

[HarmonyPatch(typeof(ProjectileCE), nameof(ProjectileCE.Impact), typeof(Thing))]
[HarmonyPatch(typeof(BulletCE), nameof(BulletCE.Impact), typeof(Thing))]
public static class ProjectileCE_BulletCE_Impact
{
    private static bool Prefix(ProjectileCE __instance, Thing hitThing)
    {
        if (hitThing is Pawn pawn)
        {
            IEnumerable<Apparel> enumerable = pawn.apparel?.WornApparel;
            foreach (var item in enumerable ?? [])
            {
                if (item is MParmorCore_Red mParmorCore_Red && mParmorCore_Red.TryReboundProjectiles(__instance))
                {
                    return false;
                }
            }
        }

        foreach (var thing in __instance.ExactPosition.ToIntVec3().GetThingList(__instance.Map))
        {
            if (thing is Pawn pawn2 && pawn2.GetMParmorCore(out var core) &&
                core is MParmorCore_Red mParmorCore_Red2 && mParmorCore_Red2.TryReboundProjectiles(__instance))
            {
                return false;
            }
        }

        return true;
    }
}