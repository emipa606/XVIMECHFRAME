using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using Verse;

namespace Tower_WeaponDrawFrame;

[UsedImplicitly]
[StaticConstructorOnStartup]
public class PatchMain
{
    static PatchMain()
    {
        new Harmony("XF.Weapon_HarmonyPatch").PatchAll(Assembly.GetExecutingAssembly());
    }
}