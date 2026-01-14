using System.Collections.Generic;
using MParmorLibrary.Patches;
using Verse;

namespace MParmorLibrary;

public class DamageWorker_WithoutShake : DamageWorker_AddInjury
{
    public override void ExplosionStart(Explosion explosion, List<IntVec3> cellsToAffect)
    {
    }

    public override void ExplosionAffectCell(Explosion explosion, IntVec3 c, List<Thing> damagedThings,
        List<Thing> ignoredThings, bool canThrowMotes)
    {
        FleckMaker_ThrowDustPuff.throwPuff = false;
        base.ExplosionAffectCell(explosion, c, damagedThings, ignoredThings, canThrowMotes);
        FleckMaker_ThrowDustPuff.throwPuff = true;
    }
}