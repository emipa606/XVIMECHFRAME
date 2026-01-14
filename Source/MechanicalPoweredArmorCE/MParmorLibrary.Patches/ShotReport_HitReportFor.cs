using HarmonyLib;
using MParmorLibrary.SingleObject;
using Verse;

namespace MParmorLibrary.Patches;

[HarmonyPatch(typeof(ShotReport), nameof(ShotReport.HitReportFor), typeof(Thing), typeof(Verb),
    typeof(LocalTargetInfo))]
public static class ShotReport_HitReportFor
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