using RimWorld;
using Verse;

namespace MParmorLibrary.Patches;

public static class FixNewFaction
{
    public static void FixNewFaction_AntiMParmor()
    {
        var factionDef = FactionDef.Named("XFMParmor_AntiMParmor");
        if (Find.FactionManager.FirstFactionOfDef(factionDef) != null)
        {
            return;
        }

        var faction = FactionGenerator.NewGeneratedFaction(new FactionGeneratorParms(factionDef));
        Find.FactionManager.Add(faction);
    }
}