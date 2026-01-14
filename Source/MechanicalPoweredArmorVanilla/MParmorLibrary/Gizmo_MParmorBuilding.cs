using UnityEngine;
using Verse;

namespace MParmorLibrary;

[StaticConstructorOnStartup]
public class Gizmo_MParmorBuilding : Gizmo
{
    public MParmorBuilding core;

    public MParmorT_HealthTracker healthTracker;

    public MParmorT_PowerTracker powerTracker;

    public Gizmo_MParmorBuilding()
    {
        Order = -1000f;
    }

    public override float GetWidth(float maxWidth)
    {
        return 215f;
    }

    public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
    {
        var rect = new Rect(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
        if (Mouse.IsOver(rect))
        {
            TipSignal tip = "";
            ref var text = ref tip.text;
            text += "XFMParmor_Gizmo_MParmor_A".Translate(healthTracker.Machine.ToString("0.#"),
                healthTracker.MachineMax.ToString());
            tip.text += "\n";
            ref var text2 = ref tip.text;
            text2 += "XFMParmor_Gizmo_MParmor_B".Translate(powerTracker.ElectiricityLabel);
            if (core.ModulesTracker.AnyModules)
            {
                var textOnGizmo = core.ModulesTracker.TextOnGizmo;
                if (!textOnGizmo.NullOrEmpty())
                {
                    tip.text += "\n";
                    tip.text += textOnGizmo;
                }

                tip.text += "\n";
                ref var text3 = ref tip.text;
                text3 += "XFMParmor_Gizmo_MParmor_C".Translate(core.ModulesTracker.NamesOfModules);
            }

            TooltipHandler.TipRegion(rect, tip);
        }

        Widgets.DrawWindowBackground(rect);
        var rect2 = new Rect(topLeft.x + 9f, topLeft.y + 9f, 185f, 25f);
        var rect3 = new Rect(topLeft.x + 9f, topLeft.y + 44f, 185f, 25f);
        var rect4 = new Rect(topLeft.x + 199f, topLeft.y + 9f, 10f, 60f);
        var fillPercent = healthTracker.Machine / healthTracker.MachineMax;
        var electricityPercent = powerTracker.ElectricityPercent;
        ToolsLibrary.FillableBarByRot4(rect2, fillPercent, Rot4.East, Colors.machineBarTex, Colors.machineEmptyBarTex);
        ToolsLibrary.FillableBarByRot4(rect3, electricityPercent, Rot4.East, Colors.shieldBarTex,
            Colors.shieldEmptyBarTex);
        if (core.ModulesTracker.TryGetModule<Module_Supply>("Module_Supply", out var module))
        {
            var preparationPercentage = module.PreparationPercentage;
            ToolsLibrary.FillableBarByRot4(rect4, preparationPercentage, Rot4.North, Colors.supplyBarTex,
                Colors.labelUnfilledMatBlack);
        }
        else
        {
            ToolsLibrary.FillableBarByRot4(rect4, 1f, Rot4.North, Colors.gray);
        }

        Text.Font = GameFont.Small;
        Text.Anchor = TextAnchor.MiddleCenter;
        Widgets.Label(rect2, $"{healthTracker.Machine:0.#} / {healthTracker.MachineMax}");
        Widgets.Label(rect3, "{0} Wh".Formatted(powerTracker.ElectiricityLabel));
        Text.Anchor = TextAnchor.UpperLeft;
        return new GizmoResult(GizmoState.Clear);
    }
}