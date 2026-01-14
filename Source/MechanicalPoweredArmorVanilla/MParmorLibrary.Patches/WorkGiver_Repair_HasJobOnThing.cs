using HarmonyLib;
using RimWorld;
using Verse;

namespace MParmorLibrary.Patches;

[HarmonyPatch(typeof(WorkGiver_Repair), nameof(WorkGiver_Repair.HasJobOnThing), typeof(Pawn), typeof(Thing),
    typeof(bool))]
public static class WorkGiver_Repair_HasJobOnThing
{
    private static void Postfix(ref bool __result, Thing t)
    {
        if (t is Wall_SelfRepairing { NotAllowFix: not false })
        {
            __result = false;
        }
    }
}