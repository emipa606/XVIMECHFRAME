using UnityEngine;
using Verse;

namespace MParmorLibrary.SkillSystem;

public class Command_SkillTarget(SkillObject skill) : Command_VerbTarget
{
    public override string TopRightLabel =>
        skill.skill.noNeedCharge ? null : (skill.energy / 60) + "/" + (skill.skill.energy / 60);

    public override Color IconDrawColor => Color.white;

    public override void GizmoUpdateOnMouseover()
    {
        verb.DrawHighlight(LocalTargetInfo.Invalid);
    }

    public override bool GroupsWith(Gizmo other)
    {
        return false;
    }

    public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
    {
        var rect = new Rect(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
        var result = base.GizmoOnGUI(topLeft, maxWidth, parms);
        if (skill.IsFinishedCharge)
        {
            return result;
        }

        var color = skill.MParmor.color;
        color.a = 0.2f;
        ToolsLibrary.FillableBarByRot4(rect, skill.ChargePercent, Rot4.North,
            SolidColorMaterials.NewSolidColorTexture(color));

        return result;
    }
}