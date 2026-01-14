using HarmonyLib;
using MParmorLibrary.SingleObject;
using Verse;

namespace MParmorLibrary.Patches;

[HarmonyPatch(typeof(Pawn), nameof(Pawn.BodySize), MethodType.Getter)]
public static class Pawn_BodySize
{
    private static void Postfix(Pawn __instance, ref float __result)
    {
        if (AcquisitionManagement.instanceBoolForBodySize && __instance.GetMParmorCore() && __result != 0f)
        {
            __result = 5f;
        }
    }
}