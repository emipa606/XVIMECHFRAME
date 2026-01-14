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
        ToolsLibrary_CEonly.LaunchProjectileRandomAngle(ThingDefOf.XFMParmor_Weapon_BloodIce, damageSource ?? pawn);
    }
}