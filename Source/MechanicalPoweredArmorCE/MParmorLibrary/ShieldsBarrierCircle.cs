using System;
using System.Collections.Generic;
using System.Reflection;
using CombatExtended;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace MParmorLibrary;

public class ShieldsBarrierCircle : ShieldObstacle, IIntercept
{
    public static List<ShieldsBarrierCircle> Cache = [];

    public bool CanIntercept(Projectile projectile, IntVec3 c)
    {
        var distance = ToolsLibrary.GetDistance(Position, c);
        return ToolsLibrary_MParmorOnly.IsUnfriendly(projectile, this) && distance is < 79.20999f and > 56.25f;
    }

    public bool TryIntercept(Projectile projectile, IntVec3 c)
    {
        var dinfo = new DamageInfo(projectile.def.projectile.damageDef, projectile.DamageAmount,
            projectile.ArmorPenetration, projectile.ExactRotation.eulerAngles.y, projectile.Launcher, null,
            projectile.EquipmentDef, DamageInfo.SourceCategory.ThingOrUnknown, projectile.intendedTarget.Thing);
        ShowImpectFleck(c.ToVector3Shifted(), dinfo.Amount);
        base.Hurt_Shield(dinfo);
        projectile.GetType().GetMethod("Impact", BindingFlags.Instance | BindingFlags.NonPublic)
            ?.Invoke(projectile, [this, false]);
        if (projectile.Spawned)
        {
            projectile.Destroy();
        }

        return true;
    }

    public bool CanIntercept(ProjectileCE projectile, IntVec3 c)
    {
        float num = (Position - c).LengthHorizontalSquared;
        return ToolsLibrary_MParmorOnly.IsUnfriendly(projectile, this) && num is < 88.35999f and > 56.25f;
    }

    public bool TryIntercept(ProjectileCE projectile, IntVec3 c)
    {
        var num = 0f;
        foreach (var item in projectile.GetProjectileDamageInfo())
        {
            num += item.Amount;
            Hurt_Shield(item);
        }

        ShowImpectFleck(c.ToVector3Shifted(), num);
        projectile.Impact(null);
        if (projectile.Spawned)
        {
            projectile.Destroy();
        }

        return true;
    }

    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        base.SpawnSetup(map, respawningAfterLoad);
        Intercepts.AddNewInstance(this);
    }

    public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
    {
        SoundDef.Named("EnergyShield_Broken").PlayOneShot(new TargetInfo(DrawPos.ToIntVec3(), Map));
        Intercepts.RemoveInstance(this);
        base.DeSpawn(mode);
    }

    public virtual void ShowImpectFleck(Vector3 drawPos, float amout)
    {
        RimWorld.SoundDefOf.EnergyShield_AbsorbDamage.PlayOneShot(new TargetInfo(drawPos.ToIntVec3(), Map));
        MakeFlecks(drawPos, amout);
    }

    private void MakeFlecks(Vector3 drawPos, float amout)
    {
        var num = Math.Max(Rand.Range(amout / 40f, 1f + (amout / 30f)), 1.5f);
        FleckMaker.Static(drawPos, Map, RimWorld.FleckDefOf.ExplosionFlash, num);
        var num2 = (int)(num / 0.5f) + Rand.Range(1, 3);
        for (var i = 0; i < num2; i++)
        {
            Map.flecks.CreateFleck(new FleckCreationData
            {
                def = FleckDefOf.XFMParmor_ShieldChip,
                scale = num * 0.5f,
                spawnPosition = drawPos,
                instanceColor = new Color(1f, 1f, Rand.Range(0.8f, 1f), Rand.Range(0.7f, 0.9f)),
                rotation = Rand.Range(0f, 360f),
                velocitySpeed = num + Rand.Range(0.2f, 0.5f),
                velocityAngle = Rand.Range(0f, 360f)
            });
        }
    }
}