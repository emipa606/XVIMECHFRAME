using System;
using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace MParmorLibrary.Patches;

[HarmonyPatch(typeof(GenLeaving), nameof(GenLeaving.DoLeavingsFor), typeof(Thing), typeof(Map), typeof(DestroyMode),
    typeof(CellRect), typeof(Predicate<IntVec3>), typeof(List<Thing>))]
public static class GenLeaving_DoLeavingsFor
{
    private static bool Prefix(Thing diedThing, Map map, CellRect leavingsRect, DestroyMode mode)
    {
        if (!GenLeaving.CanBuildingLeaveResources(diedThing, mode) ||
            diedThing is not MParmorWreckage && diedThing is not MParmorBuilding)
        {
            return true;
        }

        if (diedThing.def == ThingDefOf.XFMParmor_Wreckage_OutQuest)
        {
            var parms = new IncidentParms
            {
                target = map
            };
            ToolsLibrary.SendStandardLetter("XFMParmor_Wreckage_OutQuest_LetterA".Translate(),
                "XFMParmor_Wreckage_OutQuest_LetterB".Translate(), LetterDefOf.PositiveEvent, parms, null);
        }

        DropResources(leavingsRect.CenterCell, map);
        return false;
    }

    private static void DropResources(IntVec3 pos, Map map)
    {
        var list = new List<ThingDefCountClass>();
        var num = Rand.Range(1, 4);
        if (num == 3)
        {
            num = 1;
        }

        switch (Rand.Range(1, 5))
        {
            case 1:
                list.Add(new ThingDefCountClass(ThingDefOf.XFMParmor_PartArmor, num));
                break;
            case 2:
                list.Add(new ThingDefCountClass(ThingDefOf.XFMParmor_PartShield, num));
                break;
            case 3:
                list.Add(new ThingDefCountClass(ThingDefOf.XFMParmor_PartControl, 1));
                break;
            case 4:
                list.Add(new ThingDefCountClass(ThingDefOf.XFMParmor_PartWeapon, 1));
                break;
        }

        list.Add(new ThingDefCountClass(ThingDefOf.XFMParmor_PartEnergy, 1));
        ToolsLibrary.SpawnThingDefCount(list, pos, map);
    }
}