using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace MParmorLibrary;

public class Projectile_IntegratedMissile : Projectile_Explosive
{
    private static readonly int subMissileCount = 3;

    public static readonly float range = 14.9f;

    public static bool InHorDistOf(Vector3 Loc, Vector3 otherLoc, float maxDist)
    {
        var num = Loc.x - otherLoc.x;
        var num2 = Loc.z - otherLoc.z;
        return (num * num) + (num2 * num2) <= maxDist * maxDist;
    }

    protected override void Explode()
    {
        var list = new List<Building>();
        foreach (var item in Map.listerBuildings.allBuildingsColonist)
        {
            if (InHorDistOf(item.DrawPos, DrawPos, 14.9f))
            {
                list.Add(item);
            }
        }

        for (var num = subMissileCount; num > 0; num--)
        {
            if (list.Count > 0)
            {
                var index = Rand.Range(0, list.Count);
                ShotSubMissile(new LocalTargetInfo(list[index]));
                list.Remove(list[index]);
            }
            else
            {
                var num2 = Rand.Range(0f, MathF.PI * 2f);
                ShotSubMissile(new LocalTargetInfo(new IntVec3(new Vector2
                {
                    x = DrawPos.x + (range * (float)Math.Cos(num2)),
                    y = DrawPos.z + (range * (float)Math.Sin(num2))
                })));
            }
        }

        base.Explode();
    }

    public void ShotSubMissile(LocalTargetInfo currentTarget)
    {
        var xFMParmor_SubMissile = ThingDefOf.XFMParmor_SubMissile;
        var shootLine = new ShootLine(ExactPosition.ToIntVec3(), currentTarget.Cell);
        var thing = ThingMaker.MakeThing(ThingDefOf.XFMParmor_MechanicalArmorCore_Black);
        var drawPos = DrawPos;
        var projectile = (Projectile)GenSpawn.Spawn(xFMParmor_SubMissile, shootLine.Source, Map);
        projectile.Launch(launcher, drawPos, currentTarget.Cell, currentTarget, ProjectileHitFlags.NonTargetWorld,
            false, thing);
    }
}