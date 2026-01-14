using System;
using UnityEngine;
using Verse;

namespace MParmorLibrary;

[StaticConstructorOnStartup]
public class Gizmo_MParmorGUI : Gizmo
{
    public MParmorCore core;

    public MParmorT_HealthTracker healthTracker;

    public MParmorT_PowerTracker powerTracker;

    public Gizmo_MParmorGUI()
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
            text2 += "XFMParmor_Gizmo_MParmor_D".Translate(healthTracker.Shield.ToString("0.#"),
                healthTracker.ShieldMax.ToString());
            tip.text += "\n";
            tip.text += healthTracker.Shield >= healthTracker.ShieldMax
                ? "XFMParmor_Gizmo_MParmor_E".TranslateSimple()
                : healthTracker.ShieldRecoveryCDInt == 0
                    ? "XFMParmor_Gizmo_MParmor_F".TranslateSimple()
                    : core.CanChargeShield
                        ? "XFMParmor_Gizmo_MParmor_G".Translate(
                            (healthTracker.ShieldRecoveryCDInt / 60f).ToString("0.0"))
                        : "XFMParmor_Gizmo_MParmor_H".TranslateSimple();
            tip.text += "\n";
            ref var text3 = ref tip.text;
            text3 += "XFMParmor_Gizmo_MParmor_B".Translate(powerTracker.ElectiricityLabel);
            if (core.ModulesTracker.AnyModules)
            {
                var textOnGizmo = core.ModulesTracker.TextOnGizmo;
                if (!textOnGizmo.NullOrEmpty())
                {
                    tip.text += "\n";
                    tip.text += textOnGizmo;
                }

                tip.text += "\n";
                ref var text4 = ref tip.text;
                text4 += "XFMParmor_Gizmo_MParmor_C".Translate(core.ModulesTracker.NamesOfModules);
            }

            TooltipHandler.TipRegion(rect, tip);
        }

        Widgets.DrawWindowBackground(rect);
        var rect2 = new Rect(topLeft.x + 9f, topLeft.y + 9f, 170f, 25f);
        var rect3 = new Rect(topLeft.x + 9f, topLeft.y + 38f, 170f, 25f);
        var rect4 = new Rect(topLeft.x + 9f, topLeft.y + 66f, 170f, 2f);
        var rect5 = new Rect(topLeft.x + 184f, topLeft.y + 9f, 10f, 60f);
        var rect6 = new Rect(topLeft.x + 199f, topLeft.y + 9f, 10f, 60f);
        var fillPercent = healthTracker.Machine / healthTracker.MachineMax;
        var fillPercent2 = (healthTracker.Machine + healthTracker.DamageMachineTotal) / healthTracker.MachineMax;
        var fillPercent3 = healthTracker.Shield / healthTracker.ShieldMax;
        var fillPercent4 = (healthTracker.Shield + healthTracker.DamageShieldTotal) / healthTracker.ShieldMax;
        var fillPercent5 =
            Math.Max(
                (healthTracker.ShieldRecoveryCD - healthTracker.ShieldRecoveryCDInt) /
                (float)healthTracker.ShieldRecoveryCD, 0f);
        if (core.ModulesTracker.TryGetModule<Module_Supply>("Module_Supply", out var module))
        {
            var preparationPercentage = module.PreparationPercentage;
            ToolsLibrary.FillableBarByRot4(rect5, preparationPercentage, Rot4.North, Colors.supplyBarTex,
                Colors.labelUnfilledMatBlack);
        }
        else
        {
            ToolsLibrary.FillableBarByRot4(rect5, 1f, Rot4.North, Colors.gray);
        }

        var electricityPercent = powerTracker.ElectricityPercent;
        ToolsLibrary.FillableBarByRot4(rect2, fillPercent2, Rot4.East, Colors.machineHurtBarTex,
            Colors.machineEmptyBarTex);
        ToolsLibrary.FillableBarByRot4(rect2, fillPercent, Rot4.East, Colors.machineBarTex);
        ToolsLibrary.FillableBarByRot4(rect3, fillPercent4, Rot4.East, Colors.shieldHurtBarTex,
            Colors.shieldEmptyBarTex);
        ToolsLibrary.FillableBarByRot4(rect3, fillPercent3, Rot4.East, Colors.shieldBarTex);
        ToolsLibrary.FillableBarByRot4(rect4, fillPercent5, Rot4.East, Colors.shieldCDBarTex, Colors.shieldEmptyBarTex);
        ToolsLibrary.FillableBarByRot4(rect6, electricityPercent, Rot4.North, Colors.powerBarTex,
            Colors.labelUnfilledMatBlack);
        Text.Font = GameFont.Small;
        Text.Anchor = TextAnchor.MiddleCenter;
        Widgets.Label(rect2, $"{healthTracker.Machine:0.#} / {healthTracker.MachineMax}");
        Widgets.Label(rect3, $"{healthTracker.Shield:0.#} / {healthTracker.ShieldMax}");
        Text.Anchor = TextAnchor.UpperLeft;
        return new GizmoResult(GizmoState.Clear);
    }
}