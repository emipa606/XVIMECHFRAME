using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace MParmorLibrary;

public class Hediff_ArcticWind : HediffWithComps
{
    public const int castCD = 6000;
    private int castTime;

    public Thing damageSource;

    private List<IntVec3> TargetData =>
        field ?? (field = GenRadial.RadialCellsAround(new IntVec3(0, 0, 0), 6f, false).ToList());

    private void TimeUpdate()
    {
        var f = pawn.health.hediffSet.BleedRateTotal + 1f;
        castTime += (int)(Mathf.Pow(f, 0.5f) * 65f);
        Severity -= 0.01f;
    }

    public override void Tick()
    {
        base.Tick();
        if (!pawn.Spawned)
        {
            return;
        }

        TimeUpdate();
        while (castTime > 6000)
        {
            castTime -= 6000;
            CastBullet();
        }
    }

    private void CastBullet()
    {
        var targetCell = GetTargetCell();
        LocalTargetInfo target = targetCell;
        var list = new List<Pawn>();
        foreach (var item2 in pawn.Map.thingGrid.ThingsListAt(targetCell))
        {
            if (item2 is Pawn item)
            {
                list.Add(item);
            }
        }

        if (list.Count > 0)
        {
            target = list[Rand.Range(0, list.Count)];
        }

        LaunchProjectile(ThingDefOf.XFMParmor_Weapon_BloodIce, null, damageSource ?? pawn, target, pawn.DrawPos,
            pawn.Map, pawn);
    }

    private IntVec3 GetTargetCell()
    {
        var list = new List<IntVec3>();
        foreach (var targetDatum in TargetData)
        {
            var intVec = targetDatum;
            intVec += pawn.Position;
            if (GenSight.LineOfSight(pawn.Position, intVec, pawn.Map))
            {
                list.Add(intVec);
            }
        }

        return list[Rand.Range(0, list.Count)];
    }

    private static void LaunchProjectile(ThingDef projectileDef, Thing equipment, Thing caster, LocalTargetInfo target,
        Vector3? castPos = null, Map castMap = null, Pawn bloodSource = null)
    {
        var shootLine = new ShootLine((castPos ?? caster.DrawPos).ToIntVec3(), target.Cell);
        var bullet_BloodIce = (Bullet_BloodIce)GenSpawn.Spawn(projectileDef, shootLine.Source, castMap ?? caster.Map);
        bullet_BloodIce.bloodSource = bloodSource;
        bullet_BloodIce.Launch(caster, castPos ?? caster.DrawPos, target, target, ProjectileHitFlags.All, false,
            equipment);
    }
}