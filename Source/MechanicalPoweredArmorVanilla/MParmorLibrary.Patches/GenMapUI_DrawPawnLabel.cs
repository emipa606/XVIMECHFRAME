using System.Collections.Generic;
using HarmonyLib;
using MParmorLibrary.SingleObject;
using UnityEngine;
using Verse;

namespace MParmorLibrary.Patches;

[HarmonyPatch(typeof(GenMapUI), nameof(GenMapUI.DrawPawnLabel), typeof(Pawn), typeof(Vector2), typeof(float),
    typeof(float),
    typeof(Dictionary<string, string>), typeof(GameFont), typeof(bool), typeof(bool))]
public static class GenMapUI_DrawPawnLabel
{
    private static void Postfix(Pawn pawn, Vector2 pos)
    {
        if (pawn.apparel == null || pawn.apparel.WornApparel == null)
        {
            return;
        }

        if (AcquisitionManagement.instanceBoolForDrawPawnGUIOverlay)
        {
            if (pawn.GetMParmorCore(out var core) && core is MParmorCore_Red
                {
                    IsActiveMode: not false
                } mParmorCore_Red)
            {
                DrawActiveMode(pos, mParmorCore_Red);
            }

            return;
        }

        if (pawn.GetMParmorCore(out var core2))
        {
            DrawHealthBar(pos, core2);
            return;
        }

        foreach (var item in pawn.apparel.WornApparel)
        {
            if (item is MParmorSelfDestruct selfDestruct)
            {
                DrawSelfDestructBar(pos, selfDestruct);
            }
        }
    }

    private static void DrawSelfDestructBar(Vector2 pos, MParmorSelfDestruct selfDestruct)
    {
        var rect = new Rect(pos.x - 24f, pos.y + 10f + 6f, 48f, 12f);
        GUI.DrawTexture(rect, Colors.labelUnfilledMatDark);
        Widgets.FillableBar(rect.ContractedBy(2f), selfDestruct.stability / (float)selfDestruct.stabilityMax,
            Colors.labelMachineBarTex, null, false);
    }

    private static void DrawActiveMode(Vector2 pos, MParmorCore_Red red)
    {
        var rect = new Rect(pos.x - 16f, pos.y + 8f + 6f, 32f, 12f);
        GUI.DrawTexture(rect, Colors.labelUnfilledMat);
        Widgets.FillableBar(rect.ContractedBy(2f), red.ActivePercent, Colors.label_RedSkillB_FilledMatT, null,
            false);
    }

    private static void DrawHealthBar(Vector2 pos, MParmorCore core)
    {
        var healthTracker = core.HealthTracker;
        var rect = new Rect(pos.x - 24f, pos.y + 10f + 6f, 48f, 12f);
        GUI.DrawTexture(rect, Colors.labelUnfilledMatDark);
        if (healthTracker.HurtMachine > 0)
        {
            var image = SolidColorMaterials.NewSolidColorTexture(new Color(0.7490196f, 0f, 0f,
                healthTracker.HurtMachine / 30f * 0.7f));
            GUI.DrawTexture(rect, image);
            Widgets.FillableBar(rect.ContractedBy(2f), healthTracker.Machine / healthTracker.MachineMax,
                Colors.machineBarTex, null, false);
        }
        else
        {
            Widgets.FillableBar(rect.ContractedBy(2f), healthTracker.Machine / healthTracker.MachineMax,
                Colors.labelMachineBarTex, null, false);
        }

        rect.y += 12f;
        GUI.DrawTexture(rect, Colors.labelUnfilledMatDark);
        if (healthTracker.HurtShield > 0)
        {
            var image2 =
                SolidColorMaterials.NewSolidColorTexture(new Color(0f, 0f, 38f / 51f,
                    healthTracker.HurtShield / 30f * 0.7f));
            GUI.DrawTexture(rect, image2);
            Widgets.FillableBar(rect.ContractedBy(2f), healthTracker.Shield / healthTracker.ShieldMax,
                Colors.shieldBarTex, null, false);
        }
        else
        {
            Widgets.FillableBar(rect.ContractedBy(2f), healthTracker.Shield / healthTracker.ShieldMax,
                Colors.labelShieldBarTex, null, false);
        }
    }
}