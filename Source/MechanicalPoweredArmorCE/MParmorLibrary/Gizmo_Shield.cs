using UnityEngine;
using Verse;

namespace MParmorLibrary;

public class Gizmo_Shield : Gizmo
{
    public float fillPercent;

    public float fillPercent2 = -1f;

    public float fillPercent3 = -1f;
    public string text;

    public string text2;

    public Gizmo_Shield()
    {
        Order = -1000f;
    }

    public override float GetWidth(float maxWidth)
    {
        return 175f;
    }

    public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
    {
        var rect = new Rect(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
        Widgets.DrawWindowBackground(rect);
        var rect2 = new Rect(topLeft.x + 9f, topLeft.y + 9f, 157f, 25f);
        var rect3 = new Rect(topLeft.x + 9f, topLeft.y + 41f, 157f, 25f);
        ToolsLibrary.FillableBarByRot4(rect3, fillPercent, Rot4.East, Colors.shieldHurtBarTex,
            Colors.shieldEmptyBarTex);
        if (fillPercent2 == -1f)
        {
            fillPercent2 = fillPercent;
        }

        ToolsLibrary.FillableBarByRot4(rect3, fillPercent2, Rot4.East, Colors.shieldBarTex);
        if (fillPercent3 != -1f)
        {
            var rect4 = new Rect(topLeft.x + 9f, topLeft.y + 62f, 157f, 2f);
            ToolsLibrary.FillableBarByRot4(rect4, fillPercent3, Rot4.East, Colors.aqua, Colors.labelUnfilledMatBlack);
        }

        Text.Font = GameFont.Small;
        Widgets.Label(rect2, text);
        Text.Anchor = TextAnchor.MiddleCenter;
        Widgets.Label(rect3, text2);
        Text.Anchor = TextAnchor.UpperLeft;
        return new GizmoResult(GizmoState.Clear);
    }
}