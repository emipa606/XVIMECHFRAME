using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using RimWorld;
using Verse;

namespace MParmorLibrary.Patches;

[UsedImplicitly]
[StaticConstructorOnStartup]
public static class PatchMain
{
    static PatchMain()
    {
        new Harmony("XFMParmor_HarmonyPatch").PatchAll(Assembly.GetExecutingAssembly());
    }

    public static bool CheckPawnIsInMParmor(Pawn pawn, out Apparel core)
    {
        if (pawn.GetMParmorCore(out var core2))
        {
            core = core2;
            return true;
        }

        if (pawn.GetMParmorSelfDestruct(out var core3))
        {
            core = core3;
            return true;
        }

        core = null;
        return false;
    }
}