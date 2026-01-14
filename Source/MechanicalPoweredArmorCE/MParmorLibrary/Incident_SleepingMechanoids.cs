using RimWorld;
using RimWorld.Planet;
using Verse;

namespace MParmorLibrary;

public class Incident_SleepingMechanoids : IncidentWorker
{
    public PlanetTile tile;

    protected override bool CanFireNowSub(IncidentParms parms)
    {
        return TileFinder.TryFindNewSiteTile(out tile, 3, 10, false, null, 0, false);
    }

    protected override bool TryExecuteWorker(IncidentParms parms)
    {
        var o = SiteMaker.MakeSite(SitePartDefOf.XFMPArmor_SleepingMechanoids, tile, null, true,
            StorytellerUtility.DefaultThreatPointsNow(parms.target) * 1.3f);
        Find.WorldObjects.Add(o);
        SendStandardLetter(def.letterLabel, def.letterText, def.letterDef, parms, new LookTargets(o));
        return true;
    }
}