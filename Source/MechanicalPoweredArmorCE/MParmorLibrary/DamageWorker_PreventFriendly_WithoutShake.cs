using System.Collections.Generic;
using Verse;

namespace MParmorLibrary;

public class DamageWorker_PreventFriendly_WithoutShake : DamageWorker_AddInjury
{
    public override DamageResult Apply(DamageInfo dinfo, Thing victim)
    {
        return !ToolsLibrary_MParmorOnly.IsUnfriendly(dinfo, victim) ? new DamageResult() : base.Apply(dinfo, victim);
    }

    public override void ExplosionStart(Explosion explosion, List<IntVec3> cellsToAffect)
    {
        if (def.explosionHeatEnergyPerCell > float.Epsilon)
        {
            GenTemperature.PushHeat(explosion.Position, explosion.Map,
                def.explosionHeatEnergyPerCell * cellsToAffect.Count);
        }

        ExplosionVisualEffectCenter(explosion);
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