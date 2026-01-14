using RimWorld;
using UnityEngine;
using Verse;

namespace MParmorLibrary;

public class Gizmo_Block : Gizmo
{
    public string defaultDesc = "No description.";

    public Color defaultIconColor = Color.white;
    public string defaultLabel;

    public Texture2D icon;

    public string topRightLabel;

    public virtual string Label => defaultLabel;

    public virtual string LabelCap => Label.CapitalizeFirst();

    public virtual string TopRightLabel => topRightLabel;

    public virtual string Desc => defaultDesc;

    public virtual Color IconDrawColor => defaultIconColor;

    public virtual Texture2D BGTexture => Command.BGTex;

    public virtual Texture2D BGTextureShrunk => Command.BGTexShrunk;

    public override float GetWidth(float maxWidth)
    {
        return 75f;
    }

    public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
    {
        return GizmoOnGUIInt(new Rect(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f), parms);
    }

    public virtual GizmoResult GizmoOnGUIShrunk(Vector2 topLeft, float size, GizmoRenderParms parms)
    {
        parms.shrunk = true;
        return GizmoOnGUIInt(new Rect(topLeft.x, topLeft.y, size, size), parms);
    }

    protected virtual GizmoResult GizmoOnGUIInt(Rect butRect, GizmoRenderParms parms)
    {
        Text.Font = GameFont.Tiny;
        var white = Color.white;
        if (parms.highLight)
        {
            Widgets.DrawStrongHighlight(butRect.ExpandedBy(12f));
        }

        var material = disabled || parms.lowLight ? TexUI.GrayscaleGUI : null;
        GUI.color = parms.lowLight ? Command.LowLightBgColor : white;
        GenUI.DrawTextureWithMaterial(butRect, parms.shrunk ? BGTextureShrunk : BGTexture, material);
        GUI.color = white;
        DrawIcon(butRect, material, parms);
        var selected = false;
        GUI.color = Color.white;
        if (parms.lowLight)
        {
            GUI.color = Command.LowLightLabelColor;
        }

        if (GizmoGridDrawer.customActivator != null && GizmoGridDrawer.customActivator(this))
        {
            selected = true;
        }

        if (Widgets.ButtonInvisible(butRect))
        {
            selected = true;
        }

        if (!parms.shrunk)
        {
            var text = TopRightLabel;
            if (!text.NullOrEmpty())
            {
                var vector = Text.CalcSize(text);
                var rect = new Rect(butRect.xMax - vector.x - 2f, butRect.y + 3f, vector.x, vector.y);
                var rect2 = rect;
                rect.x -= 2f;
                rect.width += 3f;
                Text.Anchor = TextAnchor.UpperRight;
                GUI.DrawTexture(rect, TexUI.GrayTextBG);
                Widgets.Label(rect2, text);
                Text.Anchor = TextAnchor.UpperLeft;
            }

            var labelCap = LabelCap;
            if (!labelCap.NullOrEmpty())
            {
                var num = Text.CalcHeight(labelCap, butRect.width);
                var rect3 = new Rect(butRect.x, butRect.yMax - num + 12f, butRect.width, num);
                GUI.DrawTexture(rect3, TexUI.GrayTextBG);
                Text.Anchor = TextAnchor.UpperCenter;
                Widgets.Label(rect3, labelCap);
                Text.Anchor = TextAnchor.UpperLeft;
            }

            GUI.color = Color.white;
        }

        if (Mouse.IsOver(butRect))
        {
            TipSignal tip = Desc;
            if (disabled && !disabledReason.NullOrEmpty())
            {
                tip.text +=
                    ("\n\n" + "DisabledCommand".Translate() + ": " + disabledReason).Colorize(ColorLibrary.RedReadable);
            }

            TooltipHandler.TipRegion(butRect, tip);
        }

        Text.Font = GameFont.Small;
        if (!selected)
        {
            return new GizmoResult(GizmoState.Clear, null);
        }

        if (!disabled)
        {
            return new GizmoResult(GizmoState.OpenedFloatMenu, Event.current);
        }

        if (!disabledReason.NullOrEmpty())
        {
            Messages.Message(disabledReason, MessageTypeDefOf.RejectInput, false);
        }

        return new GizmoResult(GizmoState.Mouseover, null);
    }

    public virtual void DrawIcon(Rect rect, Material buttonMat, GizmoRenderParms parms)
    {
        var badTex = icon;
        if (badTex == null)
        {
            badTex = BaseContent.BadTex;
        }

        if (!disabled || parms.lowLight)
        {
            GUI.color = IconDrawColor;
        }
        else
        {
            GUI.color = IconDrawColor.SaturationChanged(0f);
        }

        if (parms.lowLight)
        {
            GUI.color = GUI.color.ToTransparent(0.6f);
        }

        Widgets.DrawTextureFitted(rect, badTex, 0.85f, Vector2.one, new Rect(0f, 0f, 1f, 1f), 0f, buttonMat);
        GUI.color = Color.white;
    }
}