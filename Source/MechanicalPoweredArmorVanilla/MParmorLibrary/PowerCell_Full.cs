using System.Text;
using UnityEngine;
using Verse;

namespace MParmorLibrary;

public class PowerCell_Full : ThingWithComps
{
    protected override void DrawAt(Vector3 drawLoc, bool flip = false)
    {
        base.DrawAt(drawLoc, flip);
        GenDraw.DrawFillableBar(new GenDraw.FillableBarRequest
        {
            center = drawLoc + new Vector3(-0.145f, 0f, -0.145f),
            size = new Vector2(0.32f, 0.03f),
            fillPercent = 1f,
            filledMat = Colors.powerBarTex_Material,
            rotation = Rot4.East
        });
    }

    public override string GetInspectString()
    {
        var stringBuilder = new StringBuilder();
        if (!base.GetInspectString().NullOrEmpty())
        {
            stringBuilder.AppendLine(base.GetInspectString());
        }

        stringBuilder.Append("XFMParmor_Building_ChargingStation_GetInspectStringB".Translate(200000));
        return stringBuilder.ToString();
    }
}