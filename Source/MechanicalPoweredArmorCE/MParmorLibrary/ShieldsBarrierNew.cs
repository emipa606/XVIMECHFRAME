using System;
using System.Collections.Generic;
using System.Reflection;
using CombatExtended;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace MParmorLibrary;

public class ShieldsBarrierNew : ShieldObstacle
{
    public override Vector3 DrawPos
    {
        get
        {
            var drawPos = base.DrawPos;
            if (Rotation == Rot4.South)
            {
                drawPos.y = AltitudeLayer.PawnUnused.AltitudeFor();
            }

            return drawPos;
        }
    }

    public bool CheckForFreeIntercept(Projectile projectile)
    {
        if (!ToolsLibrary_MParmorOnly.IsUnfriendly(projectile, holder.ConstantCaster))
        {
            return false;
        }

        var dinfo = new DamageInfo(projectile.def.projectile.damageDef, projectile.DamageAmount,
            projectile.ArmorPenetration, projectile.ExactRotation.eulerAngles.y, projectile.Launcher, null,
            projectile.EquipmentDef, DamageInfo.SourceCategory.ThingOrUnknown, projectile.intendedTarget.Thing);
        ShowImpectFleck(projectile.TrueCenter(), dinfo.Amount);
        base.Hurt_Shield(dinfo);
        projectile.GetType().GetMethod("Impact", BindingFlags.Instance | BindingFlags.NonPublic)
            ?.Invoke(projectile, [this, false]);
        if (projectile.Spawned)
        {
            projectile.Destroy();
        }

        return true;
    }

    public bool CheckForFreeIntercept(ProjectileCE projectile)
    {
        if (!ToolsLibrary_CEonly.IsUnfriendly(projectile, holder.ConstantCaster))
        {
            return false;
        }

        var num = 0f;
        foreach (var item in projectile.GetProjectileDamageInfo())
        {
            num += item.Amount;
            base.Hurt_Shield(item);
        }

        ShowImpectFleck(projectile.TrueCenter(), num);
        projectile.Impact(this);
        if (projectile.Spawned)
        {
            projectile.Destroy();
        }

        return true;
    }

    public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
    {
        foreach (var item in GetCellsInSheild(Rotation))
        {
            MakeFlecks(item.ToVector3Shifted(), 30f);
        }

        SoundDef.Named("EnergyShield_Broken").PlayOneShot(new TargetInfo(DrawPos.ToIntVec3(), Map));
        base.Destroy(mode);
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

    public static List<IntVec3> GetCellsInSheild(Rot4 rot)
    {
        if (rot == Rot4.East || rot == Rot4.West)
        {
            return
            [
                new IntVec3(0, 0, -1),
                new IntVec3(0, 0, -2),
                new IntVec3(0, 0, -3),
                new IntVec3(0, 0, 0),
                new IntVec3(0, 0, 1),
                new IntVec3(0, 0, 2),
                new IntVec3(0, 0, 3)
            ];
        }

        return
        [
            new IntVec3(-1, 0, 0),
            new IntVec3(-2, 0, 0),
            new IntVec3(-3, 0, 0),
            new IntVec3(0, 0, 0),
            new IntVec3(1, 0, 0),
            new IntVec3(2, 0, 0),
            new IntVec3(3, 0, 0)
        ];
    }
}