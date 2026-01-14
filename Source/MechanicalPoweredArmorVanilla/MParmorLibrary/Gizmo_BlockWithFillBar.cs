using UnityEngine;
using Verse;

namespace MParmorLibrary;

public class Gizmo_BlockWithFillBar : Gizmo_Block
{
    public Color fillColor;

    public float fillPercent;

    public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
    {
        var rect = new Rect(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
        var result = base.GizmoOnGUI(topLeft, maxWidth, parms);
        ToolsLibrary.FillableBarByRot4(rect, fillPercent, Rot4.North,
            SolidColorMaterials.NewSolidColorTexture(fillColor));
        return result;
    }
}