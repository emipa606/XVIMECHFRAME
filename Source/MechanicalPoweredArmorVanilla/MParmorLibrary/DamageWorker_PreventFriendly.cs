using System.Collections.Generic;
using Verse;

namespace MParmorLibrary;

public class DamageWorker_PreventFriendly : DamageWorker_AddInjury
{
    public override DamageResult Apply(DamageInfo dinfo, Thing victim)
    {
        return !ToolsLibrary_MParmorOnly.IsUnfriendly(dinfo, victim) ? new DamageResult() : base.Apply(dinfo, victim);
    }

    protected override void ExplosionDamageThing(Explosion explosion, Thing t, List<Thing> damagedThings,
        List<Thing> ignoredThings, IntVec3 cell)
    {
        if (t == null || ToolsLibrary_MParmorOnly.IsUnfriendly(explosion.instigator, t))
        {
            base.ExplosionDamageThing(explosion, t, damagedThings, ignoredThings, cell);
        }
    }
}