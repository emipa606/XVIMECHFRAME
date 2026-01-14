using HarmonyLib;
using MParmorLibrary.SingleObject;
using Verse;

namespace MParmorLibrary.Patches;

[HarmonyPatch(typeof(ThingWithComps), nameof(ThingWithComps.SpawnSetup), typeof(Map), typeof(bool))]
public static class ThingWithComps_SpawnSetup
{
    private static void Postfix(ThingWithComps __instance)
    {
        if (__instance is Projectile item)
        {
            ProjectilesStore.GetInstance().projectiles.Add(item);
        }
    }
}