using CombatExtended;
using RimWorld;
using Verse;

namespace MParmorLibrary;

public class Verb_Laser : Verb_LaunchProjectile
{
    public CompAmmoUser compAmmo;

    public ThingWithComps Equipment => CasterPawn != null ? CasterPawn.equipment.Primary : EquipmentSource;

    public virtual CompAmmoUser CompAmmo
    {
        get
        {
            if (compAmmo == null && EquipmentSource != null)
            {
                compAmmo = EquipmentSource.TryGetComp<CompAmmoUser>();
            }

            return compAmmo;
        }
    }

    private bool IsAttacking => CasterPawn.CurJobDef == RimWorld.JobDefOf.AttackStatic || WarmingUp;

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

        if (verbProps.stopBurstWithoutLos && !TryFindShootLineFromTo(caster.Position, currentTarget, out _))
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
                verbProps.defaultProjectile.projectile.GetDamageAmount(Equipment), 2000f, num, Caster, brain,
                Equipment.def);
            currentTarget.Thing.TakeDamage(dinfo);
        }
        else
        {
            currentTarget.Thing.TakeDamage(new DamageInfo(DamageDefOf.MParmor_FrostBullet,
                verbProps.defaultProjectile.projectile.GetDamageAmount(Equipment) + 2, 2000f, num, Caster, null,
                Equipment.def));
        }

        FleckMaker.ConnectingLine(vector, vector2, FleckDefOf.XFMParmor_IceLaserLine, Caster.Map);
        lastShotTick = Find.TickManager.TicksGame;
        var entry = new BattleLogEntry_RangedImpact(Caster, CurrentTarget.Thing, CurrentTarget.Thing, Equipment.def,
            verbProps.defaultProjectile, null);
        Find.BattleLog.Add(entry);
        CompAmmo.Notify_ShotFired();
        return true;
    }

    public override bool Available()
    {
        if (!base.Available())
        {
            return false;
        }

        if (CasterIsPawn)
        {
            var casterPawn = CasterPawn;
            if (casterPawn.Faction != Faction.OfPlayer && casterPawn.mindState.MeleeThreatStillThreat &&
                casterPawn.mindState.meleeThreat.Position.AdjacentTo8WayOrInside(casterPawn.Position))
            {
                return false;
            }
        }

        if (!IsAttacking || CompAmmo is not { CanBeFiredNow: false })
        {
            return true;
        }

        var val = CompAmmo;
        val?.TryStartReload();

        return false;
    }
}