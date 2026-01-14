using System;
using HarmonyLib;
using MParmorLibrary.SingleObject;
using Verse;

namespace MParmorLibrary.Patches;

[HarmonyPatch(typeof(TickManager), nameof(TickManager.DoSingleTick), new Type[] { })]
public static class TickManager_DoSingleTick
{
    private static bool Prefix()
    {
        AcquisitionManagement.GetInstance().Tick();
        return true;
    }
}