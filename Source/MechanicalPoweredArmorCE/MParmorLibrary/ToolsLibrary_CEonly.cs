using System;
using System.Collections.Generic;
using CombatExtended;
using MParmorLibrary.Settings;
using RimWorld;
using UnityEngine;
using Verse;

namespace MParmorLibrary;

public static class ToolsLibrary_CEonly
{
    public static List<DamageInfo> GetProjectileDamageInfo(this ProjectileCE projectile)
    {
        var list = new List<DamageInfo>();
        var projectile2 = projectile.def.projectile;
        var val = (ProjectilePropertiesCE)(projectile2 is ProjectilePropertiesCE ? projectile2 : null);
        if (val == null)
        {
            return list;
        }

        list.Add(new DamageInfo(projectile.def.projectile.damageDef, val.GetDamageAmount(1f, null), 0f,
            projectile.ExactRotation.eulerAngles.y, projectile.launcher, null, projectile.equipmentDef,
            DamageInfo.SourceCategory.ThingOrUnknown, projectile.intendedTarget.Thing));
        foreach (var item in val.secondaryDamage)
        {
            list.Add(item.GetDinfo());
        }

        return list;
    }

    public static void LaunchProjectile(ThingDef projectile, Thing equipment, Thing caster, LocalTargetInfo targetInfo,
        Vector3? castPos = null)
    {
        //IL_0037: Unknown result type (might be due to invalid IL or missing references)
        //IL_003d: Expected O, but got Unknown
        //IL_00e4: Unknown result type (might be due to invalid IL or missing references)
        //IL_00e9: Unknown result type (might be due to invalid IL or missing references)
        //IL_00f3: Unknown result type (might be due to invalid IL or missing references)
        //IL_0177: Unknown result type (might be due to invalid IL or missing references)
        var shootLine = new ShootLine((castPos ?? caster.DrawPos).ToIntVec3(), targetInfo.Cell);
        var val = (ProjectileCE)ThingMaker.MakeThing(projectile);
        GenSpawn.Spawn(val, castPos?.ToIntVec3() ?? caster.Position, caster.Map);
        val.minCollisionDistance = 1f;
        val.castShadow = Rand.Value < 0.2f;
        val.logMisses = false;
        var speed = projectile.projectile.speed;
        var magnitude = (shootLine.Dest - shootLine.Source).ToVector3Shifted().magnitude;
        float num;
        if (!targetInfo.HasThing)
        {
            num = 0f;
        }
        else
        {
            var val2 = new CollisionVertical(targetInfo.Thing);
            num = val2.MiddleHeight;
        }

        var num2 = num - new CollisionVertical(caster).shotHeight;
        var flyOverhead = projectile.projectile.flyOverhead;
        var projectile2 = projectile.projectile;
        var shotAngle = CE_Utility.GetShotAngle(speed, magnitude, num2, flyOverhead,
            (float)((ProjectilePropertiesCE)(projectile2 is ProjectilePropertiesCE ? projectile2 : null))!.Gravity);
        val.Launch(caster, (castPos ?? caster.DrawPos).V3ToV2(), shotAngle,
            180f - ((castPos ?? caster.TrueCenter()) - targetInfo.CenterVector3).AngleFlat(),
            new CollisionVertical(caster).shotHeight, projectile.projectile.speed, equipment);
    }

    public static void LaunchProjectileRandomAngle(ThingDef projectile, Thing caster, Vector3? castPos = null)
    {
        //IL_0008: Unknown result type (might be due to invalid IL or missing references)
        //IL_000e: Expected O, but got Unknown
        //IL_00af: Unknown result type (might be due to invalid IL or missing references)
        var val = (ProjectileCE)ThingMaker.MakeThing(projectile);
        GenSpawn.Spawn(val, castPos?.ToIntVec3() ?? caster.Position, caster.Map);
        val.minCollisionDistance = 1f;
        val.castShadow = Rand.Value < 0.2f;
        val.logMisses = false;
        var floatRange = new FloatRange(0.5f, 0.8f);
        val.Launch(caster, (castPos ?? caster.DrawPos).V3ToV2(), floatRange.RandomInRange * (MathF.PI / 180f) * 0.1f,
            Rand.Range(0, 360), new CollisionVertical(caster).shotHeight, projectile.projectile.speed * 0.25f);
    }

    public static bool IsUnfriendly(ProjectileCE a, Thing b)
    {
        if (a.launcher == null)
        {
            return true;
        }

        if (a.launcher == b)
        {
            return false;
        }

        if (Main.Instance.settings.forceFriendlyFire)
        {
            return true;
        }

        return b == null || a.launcher.Faction == null || b.Faction == null || a.launcher.Faction.HostileTo(b.Faction);
    }
}