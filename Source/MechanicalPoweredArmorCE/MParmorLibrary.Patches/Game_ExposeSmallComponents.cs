using HarmonyLib;
using MParmorLibrary.SingleObject;
using Verse;

namespace MParmorLibrary.Patches;

[HarmonyPatch(typeof(Game), "ExposeSmallComponents", [])]
public static class Game_ExposeSmallComponents
{
    private static void Postfix()
    {
        SingleObjectLibrary.NewExposeData();
    }
}