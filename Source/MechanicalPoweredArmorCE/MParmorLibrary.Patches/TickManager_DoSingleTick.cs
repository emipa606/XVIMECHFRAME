using HarmonyLib;
using MParmorLibrary.SingleObject;
using Verse;

namespace MParmorLibrary.Patches;

[HarmonyPatch(typeof(TickManager), nameof(TickManager.DoSingleTick), [])]
public static class TickManager_DoSingleTick
{
    private static void Prefix()
    {
        AcquisitionManagement.GetInstance().Tick();
    }
}