using HarmonyLib;
using MParmorLibrary.SingleObject;
using Verse;

namespace MParmorLibrary.Patches;

[HarmonyPatch(typeof(Projectile), "Tick")]
public static class Projectile_Tick
{
    private static void Prefix()
    {
        AcquisitionManagement.instanceBoolForBodySize = true;
    }

    private static void Postfix()
    {
        AcquisitionManagement.instanceBoolForBodySize = false;
    }
}