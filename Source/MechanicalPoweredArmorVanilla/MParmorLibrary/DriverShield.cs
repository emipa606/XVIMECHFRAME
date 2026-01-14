using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace MParmorLibrary;

[StaticConstructorOnStartup]
public class DriverShield : Apparel
{
    public float shield;

    public float shieldMax;

    public DriverShield()
    {
        shieldMax = shield = Rand.Range(100, 250);
    }

    public static void AddShield(Pawn pawn, float energy)
    {
        var driverShield = (DriverShield)ThingMaker.MakeThing(ThingDefOf.XFMParmor_DriverShield);
        pawn.apparel.Wear(driverShield, false, true);
        driverShield.shield = energy;
        driverShield.shieldMax = energy;
    }

    public override IEnumerable<Gizmo> GetWornGizmos()
    {
        foreach (var wornGizmo in base.GetWornGizmos())
        {
            yield return wornGizmo;
        }

        yield return new Gizmo_Shield
        {
            text = Label,
            text2 = $"{shield:0.#}/{shieldMax:0}",
            fillPercent = shield / shieldMax
        };
    }

    public override bool CheckPreAbsorbDamage(DamageInfo dinfo)
    {
        if (!dinfo.Def.harmsHealth)
        {
            return true;
        }

        shield -= dinfo.Amount;
        if (dinfo.IsMeleeDamage(Wearer))
        {
            shield -= dinfo.Amount * 2f;
        }

        AbsorbedDamage(dinfo);
        if (shield <= 0f)
        {
            Break();
        }

        return true;
    }

    private void AbsorbedDamage(DamageInfo dinfo)
    {
        SoundDef.Named("EnergyShield_Broken").PlayOneShot(new TargetInfo(Wearer.Position, Wearer.Map));
        var v = Vector3Utility.HorizontalVectorFromAngle(dinfo.Angle);
        var loc = Wearer.TrueCenter() + (v.RotatedBy(180f) * 0.5f);
        var num = Mathf.Min(10f, 2f + (dinfo.Amount / 10f));
        FleckMaker.Static(loc, Wearer.Map, RimWorld.FleckDefOf.ExplosionFlash, num);
        var num2 = (int)num;
        for (var i = 0; i < num2; i++)
        {
            Wearer.Map.flecks.CreateFleck(new FleckCreationData
            {
                def = FleckDefOf.XFMParmor_ShieldChip,
                scale = 0.7f,
                spawnPosition = Wearer.DrawPos,
                instanceColor = new Color(1f, 1f, Rand.Range(0.8f, 1f), Rand.Range(0.7f, 0.9f)),
                rotation = Rand.Range(0f, 360f),
                velocitySpeed = Rand.Range(0.2f, 0.5f),
                velocityAngle = Rand.Range(0f, 360f)
            });
        }
    }

    private void Break()
    {
        SoundDef.Named("EnergyShield_Broken").PlayOneShot(new TargetInfo(Wearer.Position, Wearer.Map));
        FleckMaker.Static(Wearer.TrueCenter(), Wearer.Map, RimWorld.FleckDefOf.ExplosionFlash, 12f);
        for (var i = 0; i < 6; i++)
        {
            Wearer.Map.flecks.CreateFleck(new FleckCreationData
            {
                def = FleckDefOf.XFMParmor_ShieldChip,
                scale = 0.7f,
                spawnPosition = Wearer.DrawPos,
                instanceColor = new Color(1f, 1f, Rand.Range(0.8f, 1f), Rand.Range(0.7f, 0.9f)),
                rotation = Rand.Range(0f, 360f),
                velocitySpeed = Rand.Range(0.2f, 0.5f),
                velocityAngle = Rand.Range(0f, 360f)
            });
        }

        Wearer.apparel.Remove(this);
    }

    public override void DrawWornExtras()
    {
        var wearer = Wearer;
        if (!wearer.Spawned || !wearer.InAggroMentalState && !wearer.Drafted &&
            (!wearer.Faction.HostileTo(Faction.OfPlayer) || wearer.IsPrisoner))
        {
            return;
        }

        var drawPos = Wearer.Drawer.DrawPos;
        drawPos.y = AltitudeLayer.Blueprint.AltitudeFor();
        var s = new Vector3(2.21f, 1f, 2.21f);
        var matrix = default(Matrix4x4);
        matrix.SetTRS(drawPos, Quaternion.AngleAxis(0f, Vector3.up), s);
        Graphics.DrawMesh(MeshPool.plane10, matrix, MaterialOf.DriverShield, 0);
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref shield, "energy", -1f);
        Scribe_Values.Look(ref shieldMax, "enerygyMax", -1f);
    }
}