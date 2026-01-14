using MParmorLibrary.SingleObject;
using RimWorld;
using UnityEngine;
using Verse;

namespace MParmorLibrary;

public class IncidentWorker_WreckageFall : IncidentWorker
{
    protected override bool CanFireNowSub(IncidentParms parms)
    {
        var map = (Map)parms.target;
        return !AcquisitionManagement.GetInstance().firstPast && TryFindCell(out _, map);
    }

    protected override bool TryExecuteWorker(IncidentParms parms)
    {
        var map = (Map)parms.target;
        if (!TryFindCell(out var cell, map))
        {
            return false;
        }

        SkyfallerMaker.SpawnSkyfaller(ThingDefOf.XFMParmor_Wreckage_Falling, ThingDefOf.XFMParmor_Wreckage_Misc, cell,
            map);
        SendStandardLetter(def.letterLabel, def.letterText, def.letterDef, parms, new TargetInfo(cell, map));
        return true;
    }

    private static bool TryFindCell(out IntVec3 cell, Map map)
    {
        var maxMineables = ThingSetMaker_Meteorite.MineablesCountRange.max;
        return CellFinderLoose.TryFindSkyfallerCell(ThingDefOf.XFMParmor_Wreckage_Falling, map,
            TerrainAffordanceDefOf.Walkable, out cell, 10, default, -1, false, false, false, false, true, true,
            delegate(IntVec3 x)
            {
                var num = Mathf.CeilToInt(Mathf.Sqrt(maxMineables)) + 2;
                var cellRect = CellRect.CenteredOn(x, num, num);
                var num2 = 0;
                foreach (var item in cellRect)
                {
                    if (item.InBounds(map) && item.Standable(map))
                    {
                        num2++;
                    }
                }

                return num2 >= maxMineables;
            });
    }
}