using CombatExtended;
using Verse;

namespace MParmorLibrary;

public class Verb_Shoot_PulseRifle : Verb_ShootCE
{
    private int bulletNumber;

    public override ThingDef Projectile => bulletNumber >= 3 ? ThingDefOf.XFMParmor_PulseRifle_Grenade : Projectile;

    public override void WarmupComplete()
    {
        bulletNumber = 0;
        base.WarmupComplete();
    }

    public override bool TryCastShot()
    {
        if (!base.TryCastShot())
        {
            return false;
        }

        bulletNumber++;
        return true;
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref bulletNumber, "bulletNumber");
    }
}