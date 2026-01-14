using RimWorld;
using Verse;

namespace MParmorLibrary;

public class Verb_Laser : Verb_LaunchProjectile
{
    public ThingWithComps Equipment => CasterPawn != null ? CasterPawn.equipment.Primary : EquipmentSource;

    protected override bool TryCastShot()
    {
        if (!currentTarget.HasThing)
        {
            return false;
        }

        if (currentTarget.Thing.Map != caster.Map)
        {
            return false;
        }

        var foundLine = TryFindShootLineFromTo(caster.Position, currentTarget, out _);
        if (verbProps.stopBurstWithoutLos && !foundLine)
        {
            return false;
        }

        var vector = Caster.TrueCenter();
        var vector2 = currentTarget.Thing.TrueCenter();
        var num = (vector - vector2).AngleFlat();
        if (currentTarget.Thing is Pawn pawn)
        {
            var brain = pawn.health.hediffSet.GetBrain();
            var dinfo = new DamageInfo(DamageDefOf.MParmor_FrostBullet,
                verbProps.defaultProjectile.projectile.GetDamageAmount(Equipment), 2f, num, Caster, brain,
                Equipment.def);
            currentTarget.Thing.TakeDamage(dinfo);
        }
        else
        {
            currentTarget.Thing.TakeDamage(new DamageInfo(DamageDefOf.MParmor_FrostBullet,
                verbProps.defaultProjectile.projectile.GetDamageAmount(Equipment) + 2, 2f, num, Caster, null,
                Equipment.def));
        }

        FleckMaker.ConnectingLine(vector, vector2, FleckDefOf.XFMParmor_IceLaserLine, Caster.Map);
        lastShotTick = Find.TickManager.TicksGame;
        var entry = new BattleLogEntry_RangedImpact(Caster, CurrentTarget.Thing, CurrentTarget.Thing, Equipment.def,
            verbProps.defaultProjectile, null);
        Find.BattleLog.Add(entry);
        return true;
    }

    public override bool Available()
    {
        if (!base.Available())
        {
            return false;
        }

        if (!CasterIsPawn)
        {
            return true;
        }

        var casterPawn = CasterPawn;
        return casterPawn.Faction == Faction.OfPlayer || !casterPawn.mindState.MeleeThreatStillThreat ||
               !casterPawn.mindState.meleeThreat.Position.AdjacentTo8WayOrInside(casterPawn.Position);
    }
}