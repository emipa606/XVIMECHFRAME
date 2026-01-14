using HarmonyLib;
using MParmorLibrary.SingleObject;
using Verse;

namespace MParmorLibrary.Patches;

[HarmonyPatch(typeof(ThingWithComps), nameof(ThingWithComps.DeSpawn), typeof(DestroyMode))]
public static class ThingWithComps_DeSpawn
{
    private static void Postfix(ThingWithComps __instance)
    {
        if (__instance is Projectile item)
        {
            ProjectilesStore.GetInstance().projectiles.Remove(item);
        }
    }
}