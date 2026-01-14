using System.Collections.Generic;
using CombatExtended;
using HarmonyLib;
using RimWorld;
using Verse;

namespace MParmorLibrary.Patches;

[StaticConstructorOnStartup]
[HarmonyPatch(typeof(CompSuppressable), nameof(CompSuppressable.AddSuppression))]
public static class CompSuppressable_AddSuppression
{
    private static bool Prefix(CompSuppressable __instance)
    {
        IEnumerable<Apparel> enumerable = (__instance.parent as Pawn)?.apparel?.WornApparel;
        foreach (var item in enumerable ?? [])
        {
            if (item is IAntiSuppressable { CanAntiSuppressable: not false })
            {
                return false;
            }
        }

        return true;
    }
}