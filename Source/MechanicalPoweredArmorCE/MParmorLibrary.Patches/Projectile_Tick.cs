using HarmonyLib;
using MParmorLibrary.SingleObject;
using Verse;

namespace MParmorLibrary.Patches;

[HarmonyPatch(typeof(Projectile), "Tick")]
public static class Projectile_Tick
{
    private static bool Prefix()
    {
        AcquisitionManagement.instanceBoolForBodySize = true;
        return true;
    }

    private static void Postfix()
    {
        AcquisitionManagement.instanceBoolForBodySize = false;
    }
}