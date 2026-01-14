using System.Text;
using UnityEngine;
using Verse;

namespace MParmorLibrary;

public class PowerCell : ThingWithComps
{
    public int batteryLevel;

    protected override void DrawAt(Vector3 drawLoc, bool flip = false)
    {
        base.DrawAt(drawLoc, flip);
        GenDraw.DrawFillableBar(new GenDraw.FillableBarRequest
        {
            center = drawLoc + new Vector3(-0.145f, 0f, -0.145f),
            size = new Vector2(0.32f, 0.03f),
            fillPercent = batteryLevel / 20000000f,
            filledMat = Colors.powerBarTex_Material,
            rotation = Rot4.East
        });
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref batteryLevel, "batteryLevel");
    }

    public override string GetInspectString()
    {
        var stringBuilder = new StringBuilder();
        if (!base.GetInspectString().NullOrEmpty())
        {
            stringBuilder.AppendLine(base.GetInspectString());
        }

        stringBuilder.Append(
            "XFMParmor_Building_ChargingStation_GetInspectStringB".Translate((int)(batteryLevel / 100f)));
        return stringBuilder.ToString();
    }
}