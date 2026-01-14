using UnityEngine;
using Verse;

namespace MParmorLibrary;

public class Module_SolarPower : Module
{
    public int RoofedCells
    {
        get
        {
            if (Instance is not MParmorBuilding mParmorBuilding)
            {
                return int.MaxValue;
            }

            var num = 0;
            foreach (var item in ToolsLibrary.NineCellsField)
            {
                if (mParmorBuilding.Map.roofGrid.Roofed(item + mParmorBuilding.Position))
                {
                    num++;
                }
            }

            return num;
        }
    }

    public float PowerOutput
    {
        get
        {
            float num = 1200 - (150 * RoofedCells);
            if (num < 0f)
            {
                return 0f;
            }

            return num * Building.Map.skyManager.CurSkyGlow;
        }
    }

    public override string TextOnGizmo
    {
        get
        {
            if (Instance is not MParmorBuilding)
            {
                return null;
            }

            string text = "XFMParmor_Module_SolarPower_A".Translate(PowerOutput.ToString("#####0"));
            var roofedCells = RoofedCells;
            if (roofedCells > 0)
            {
                text += "XFMParmor_Module_SolarPower_B".Translate(roofedCells.ToString());
            }

            return text;
        }
    }

    public override void DrawBuilding(Vector3 vector3)
    {
        var pos = vector3;
        pos.y += AltitudeLayer.Building.AltitudeFor();
        var s = new Vector3(4.5f, 1f, 4.5f);
        var matrix = default(Matrix4x4);
        matrix.SetTRS(pos, Quaternion.AngleAxis(0f, Vector3.up), s);
        Graphics.DrawMesh(MeshPool.plane10, matrix, MaterialOf.SolarPower_Plant, 0);
        var pos2 = vector3;
        pos2.y = AltitudeLayer.MoteLow.AltitudeFor();
        var s2 = new Vector3(4.5f, 1f, 4.5f);
        var matrix2 = default(Matrix4x4);
        matrix2.SetTRS(pos2, Quaternion.AngleAxis(0f, Vector3.up), s2);
        Graphics.DrawMesh(MeshPool.plane10, matrix2, MaterialOf.SolarPower_Bracket, 0);
    }

    public override void TickBuilding()
    {
        Building.PowerTracker.ChargeBatteryExactly((int)(PowerOutput * 100f / 2500f));
    }
}