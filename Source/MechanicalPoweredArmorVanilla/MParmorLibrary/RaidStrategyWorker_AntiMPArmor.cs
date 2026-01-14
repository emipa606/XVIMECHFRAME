using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace MParmorLibrary;

public class RaidStrategyWorker_AntiMPArmor : RaidStrategyWorker
{
    protected override LordJob MakeLordJob(IncidentParms parms, Map map, List<Pawn> pawns, int raidSeed)
    {
        return new LordJob_AntiMParmor();
    }
}