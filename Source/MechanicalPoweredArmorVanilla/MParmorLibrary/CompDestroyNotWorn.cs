using RimWorld;
using Verse;

namespace MParmorLibrary;

public class CompDestroyNotWorn : ThingComp
{
    public override void PostSpawnSetup(bool respawningAfterLoad)
    {
        base.PostSpawnSetup(respawningAfterLoad);
        if (parent is Apparel { Wearer: null } apparel)
        {
            apparel.Destroy();
        }

        foreach (var comp in parent.GetComps<CompEquippable>())
        {
            if (comp.PrimaryVerb.CasterPawn == null)
            {
                parent.Destroy();
            }
        }
    }
}