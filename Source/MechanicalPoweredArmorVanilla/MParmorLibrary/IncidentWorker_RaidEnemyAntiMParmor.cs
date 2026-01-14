using RimWorld;
using Verse;

namespace MParmorLibrary;

public class IncidentWorker_RaidEnemyAntiMParmor : IncidentWorker_RaidEnemy
{
    protected override bool CanFireNowSub(IncidentParms parms)
    {
        if (parms.target is Map map && (ToolsLibrary_MParmorOnly.GetMParmorBuilding(map).Count != 0 ||
                                        ToolsLibrary_MParmorOnly.GetMParmor(map).Count != 0))
        {
            return true;
        }

        return false;
    }

    protected virtual bool FactionCanBeGroupSource(Faction f, Map map, bool desperate = false)
    {
        return f.def.defName == "XFMParmor_AntiMParmor";
    }

    public override void ResolveRaidArriveMode(IncidentParms parms)
    {
        parms.raidArrivalMode = PawnsArrivalModeDefOf.EdgeWalkIn;
    }

    protected override bool TryResolveRaidFaction(IncidentParms parms)
    {
        parms.faction = Find.FactionManager.FirstFactionOfDef(FactionDef.Named("XFMParmor_AntiMParmor"));
        return true;
    }

    public override void ResolveRaidStrategy(IncidentParms parms, PawnGroupKindDef groupKind)
    {
        parms.raidStrategy = RaidStrategyDefOf.XFMParmor_RaidStrategy_AntiMPArmor;
    }
}