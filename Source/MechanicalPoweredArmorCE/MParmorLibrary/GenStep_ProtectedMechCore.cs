using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace MParmorLibrary;

public class GenStep_ProtectedMechCore : GenStep_SleepingMechanoids
{
    public override void Generate(Map map, GenStepParams parms)
    {
        if (!SiteGenStepUtility.TryFindRootToSpawnAroundRectOfInterest(out var rectToDefend,
                out var singleCellToSpawnNear, map))
        {
            return;
        }

        var list = new List<Pawn>();
        foreach (var item in GeneratePawns(parms, map))
        {
            if (!SiteGenStepUtility.TryFindSpawnCellAroundOrNear(rectToDefend, singleCellToSpawnNear, map,
                    out var spawnCell))
            {
                Find.WorldPawns.PassToWorld(item);
                break;
            }

            GenSpawn.Spawn(item, spawnCell, map);
            list.Add(item);
        }

        if (!list.Any())
        {
            return;
        }

        foreach (var item2 in list)
        {
            var comp = item2.GetComp<CompWakeUpDormant>();
            comp?.wakeUpIfTargetClose = true;
        }

        LordMaker.MakeNewLord(Faction.OfMechanoids, new LordJob_SleepThenAssaultColony(Faction.OfMechanoids), map,
            list);
        SendMechanoidsToSleepImmediately(list);
        GenSpawn.Spawn(ThingDefOf.XFMParmor_Wreckage_OutQuest, singleCellToSpawnNear, map);
    }

    private IEnumerable<Pawn> GeneratePawns(GenStepParams parms, Map map)
    {
        var points = parms.sitePart != null ? parms.sitePart.parms.threatPoints : defaultPointsRange.RandomInRange;
        var pawnGroupMakerParms = new PawnGroupMakerParms
        {
            groupKind = PawnGroupKindDefOf.Combat,
            tile = map.Tile,
            faction = Faction.OfMechanoids,
            points = points
        };
        if (parms.sitePart != null)
        {
            pawnGroupMakerParms.seed = SleepingMechanoidsSitePartUtility.GetPawnGroupMakerSeed(parms.sitePart.parms);
        }

        return PawnGroupMakerUtility.GeneratePawns(pawnGroupMakerParms);
    }
}