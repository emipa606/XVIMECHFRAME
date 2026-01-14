using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace MParmorLibrary;

public class Verb_ShootClustered : Verb_Shoot
{
    private int bulletNumber;

    private MParmorCore_Carrier core;

    public override void WarmupComplete()
    {
        bulletNumber = 0;
        base.WarmupComplete();
    }

    protected override bool TryCastShot()
    {
        var castShot = base.TryCastShot();
        if (!castShot)
        {
            return false;
        }

        if (bulletNumber % 2 == 0)
        {
            TryCastDroneShot();
        }

        bulletNumber++;

        return true;
    }

    private void TryCastDroneShot()
    {
        if (core == null)
        {
            CasterPawn.GetMParmorCore(out var mParmorCore);
            core = mParmorCore as MParmorCore_Carrier;
        }

        if (core == null)
        {
            return;
        }

        foreach (var item in core.GetDronesPosition())
        {
            var vector = item + Caster.TrueCenter();
            ToolsLibrary.LaunchProjectile(Projectile, EquipmentSource, Caster,
                DirectHitTarget(vector, CurrentTarget)
                    ? CurrentTarget
                    : new LocalTargetInfo(CurrentTarget.Cell + ToolsLibrary.EightCells().RandomElement()), vector);

            FleckMaker.Static(caster.Position, caster.Map, RimWorld.FleckDefOf.ShotFlash,
                verbProps.muzzleFlashScale);
        }
    }

    private static bool DirectHitTarget(Vector3 pos, LocalTargetInfo localTarget)
    {
        var distance = ToolsLibrary.GetDistance(pos, localTarget.CenterVector3, true);
        distance -= 10f;
        var num = 40 + Math.Max(0, 100 - (int)(distance * 100f / 15f));
        return Rand.Range(1, 101) <= num;
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref bulletNumber, "bulletNumber");
    }
}