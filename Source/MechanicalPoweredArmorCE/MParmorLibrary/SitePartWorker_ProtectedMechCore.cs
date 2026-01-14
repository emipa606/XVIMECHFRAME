using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace MParmorLibrary;

public class SitePartWorker_ProtectedMechCore : SitePartWorker_SleepingMechanoids
{
    public override string GetPostProcessedThreatLabel(Site site, SitePart sitePart)
    {
        return $"{def.label}: " +
               "KnownSiteThreatEnemyCountAppend".Translate(GetMechanoidsCount(site, sitePart.parms),
                   "Enemies".Translate()) + "," + "XFMParmor_SitePartWorker_ProtectedMechCoreA".Translate();
    }

    private static int GetMechanoidsCount(Site site, SitePartParams parms)
    {
        return PawnGroupMakerUtility.GeneratePawnKindsExample(new PawnGroupMakerParms
        {
            tile = site.Tile,
            faction = Faction.OfMechanoids,
            groupKind = PawnGroupKindDefOf.Combat,
            points = parms.threatPoints,
            seed = SleepingMechanoidsSitePartUtility.GetPawnGroupMakerSeed(parms)
        }).Count();
    }
}