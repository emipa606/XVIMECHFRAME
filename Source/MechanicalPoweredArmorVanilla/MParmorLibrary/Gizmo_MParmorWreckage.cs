using UnityEngine;
using Verse;

namespace MParmorLibrary;

[StaticConstructorOnStartup]
public class Gizmo_MParmorWreckage : Gizmo
{
    private static readonly Texture2D shieldHurtBarTex =
        SolidColorMaterials.NewSolidColorTexture(new Color(0f, 0.6f, 1f));

    private static readonly Texture2D shieldEmptyBarTex =
        SolidColorMaterials.NewSolidColorTexture(new Color(0f, 0.12f, 0.25f));

    public string driverName = "";
    public int stability;

    public int stabilityMax;

    public Gizmo_MParmorWreckage()
    {
        Order = -1000f;
    }

    public override float GetWidth(float maxWidth)
    {
        return 235f;
    }

    public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
    {
        var rect = new Rect(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
        var rect2 = rect.ContractedBy(6f);
        rect2.xMin += 55f;
        Widgets.DrawWindowBackground(rect);
        var rect3 = rect2;
        rect3.yMin = rect2.y + (rect2.height * 0.2f) - (rect2.height * 0.05f);
        rect3.height = rect2.height * 0.4f;
        var rect4 = rect3;
        rect4.xMin -= 55f;
        var rect5 = rect2;
        rect5.yMin = rect2.y + (rect2.height * 0.2f) + rect3.height;
        rect5.height = rect2.height / 2f * 0.8f;
        var rect6 = rect5;
        rect6.xMin -= 55f;
        Text.Font = GameFont.Tiny;
        var num = stability / (float)stabilityMax;
        Text.Font = GameFont.Small;
        Text.Anchor = TextAnchor.MiddleCenter;
        if (stability != 0)
        {
            Widgets.FillableBar(rect5, num, shieldHurtBarTex, shieldEmptyBarTex, false);
            Widgets.Label(rect5, $"{(int)(num * 100f)}%");
        }

        Text.Anchor = TextAnchor.MiddleLeft;
        Widgets.Label(rect4, "Gizmo_MParmorWreckage_LastDriver".Translate() + driverName);
        Widgets.Label(rect6,
            stability != 0 ? "Gizmo_MParmorWreckageA".Translate() : "Gizmo_MParmorWreckageB".Translate());

        Text.Anchor = TextAnchor.UpperLeft;
        return new GizmoResult(GizmoState.Clear);
    }
}