using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace MParmorLibrary;

public class MParmorCore_Carrier : MParmorCore
{
    private const int PowerOfWorkPerTick = 51;

    private const int WorkloadPerDrone = 960;

    private const float SurroundDistance = 1f;
    private readonly Vector3?[] dronesPosition = new Vector3?[3];

    private float surroundAngle;

    private int workload;

    public int DronesCount => workload / 960;

    protected override void TickMParmor(out bool returnNow)
    {
        returnNow = false;
        DroneSurrounding();
        if (!CanChargeSkills || DronesCount >= 3)
        {
            return;
        }

        workload++;
        PowerTracker.ConsumeBatteryExactly(51);
    }

    public void SpendDroune()
    {
        workload -= 960;
    }

    public void RecycleDrone(float recoveryPercent)
    {
        var num = (int)(960f * recoveryPercent);
        var num2 = 2880 - workload;
        if (num < num2)
        {
            workload += num;
            return;
        }

        num -= num2;
        workload = 2880;
        PowerTracker.ChargeBatteryExactly(num * 51);
    }

    public override IEnumerable<Gizmo> GetExtraSkillGizmos()
    {
        var color = Color.white;
        color.a = 0.2f;
        yield return new Gizmo_BlockWithFillBar
        {
            defaultDesc = "XFMParmorCore_Carrier_defaultDesc".Translate(),
            defaultLabel = "XFMParmorCore_Carrier_defaultLabel".Translate(),
            topRightLabel = "{0}/3".Formatted(DronesCount.ToString()),
            fillColor = color,
            fillPercent = workload % 960 / 960f,
            icon = Texture2DOf.DroneIcon
        };
    }

    protected override void BaseMParmorDrawAt(Vector3 vector3)
    {
        base.BaseMParmorDrawAt(vector3);
        foreach (var item in GetDronesPosition())
        {
            DrawSingleDrones(item);
        }
    }

    private void DrawSingleDrones(Vector3 pos)
    {
        var pos2 = Wearer.Drawer.DrawPos + pos;
        if (pos.z > 0f)
        {
            pos2.y = AltitudeLayer.Item.AltitudeFor();
        }
        else
        {
            pos2.y = AltitudeLayer.MoteOverhead.AltitudeFor() + 1f;
        }

        var s = new Vector3(1.5f, 1f, 1.5f);
        var matrix = default(Matrix4x4);
        matrix.SetTRS(pos2, Quaternion.AngleAxis(0f, Vector3.up), s);
        Graphics.DrawMesh(MeshPool.plane10, matrix, MaterialOf.Drone, 0);
    }

    public IEnumerable<Vector3> GetDronesPosition()
    {
        for (var count = 0; count < DronesCount; count++)
        {
            if (dronesPosition[count].HasValue)
            {
                yield return dronesPosition[count].Value;
            }
        }
    }

    private void DroneSurrounding()
    {
        surroundAngle += 1f;
        if (surroundAngle > 360f)
        {
            surroundAngle -= 360f;
        }

        dronesPosition[0] = null;
        dronesPosition[1] = null;
        dronesPosition[2] = null;
        dronesPosition[0] = new Vector3((float)(Math.Sin(surroundAngle * (Math.PI / 180.0)) * 1.0), 0f,
            (float)(Math.Cos(surroundAngle * (Math.PI / 180.0)) * 1.0));
        if (DronesCount == 2)
        {
            dronesPosition[1] = new Vector3((float)(Math.Sin((surroundAngle + 180f) * (Math.PI / 180.0)) * 1.0), 0f,
                (float)(Math.Cos((surroundAngle + 180f) * (Math.PI / 180.0)) * 1.0));
        }
        else if (DronesCount >= 3)
        {
            dronesPosition[1] = new Vector3((float)(Math.Sin((surroundAngle + 120f) * (Math.PI / 180.0)) * 1.0), 0f,
                (float)(Math.Cos((surroundAngle + 120f) * (Math.PI / 180.0)) * 1.0));
            dronesPosition[2] = new Vector3((float)(Math.Sin((surroundAngle + 240f) * (Math.PI / 180.0)) * 1.0), 0f,
                (float)(Math.Cos((surroundAngle + 240f) * (Math.PI / 180.0)) * 1.0));
        }
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref surroundAngle, "surroundAngle");
        Scribe_Values.Look(ref workload, "workload");
    }
}