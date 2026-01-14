using HarmonyLib;
using UnityEngine;
using Verse;

namespace Tower_WeaponDrawFrame;

[HarmonyPatch(typeof(PawnRenderUtility), nameof(PawnRenderUtility.DrawEquipmentAiming), typeof(Thing), typeof(Vector3),
    typeof(float))]
public static class PawnRenderUtility_DrawEquipmentAiming
{
    private static bool Prefix(Thing eq, Vector3 drawLoc, float aimAngle)
    {
        if (eq is not ThingWithComps comps)
        {
            return true;
        }

        var compWeaponDraw = comps.TryGetComp<CompWeaponDraw>();
        if (compWeaponDraw == null)
        {
            return true;
        }

        comps.TryGetComp<CompEquippable>(out var comp);
        var pawn = (comp.parent.ParentHolder as Pawn_EquipmentTracker)?.pawn;
        if (pawn == null)
        {
            return true;
        }

        if (compWeaponDraw.showMainHandNow == null || compWeaponDraw.showMainHandNow())
        {
            MainHand(comps, drawLoc, aimAngle, compWeaponDraw, pawn);
        }

        if (compWeaponDraw.Props.offHand != null &&
            (compWeaponDraw.showOffHandNow == null || compWeaponDraw.showOffHandNow()))
        {
            OffHand(comps, drawLoc, aimAngle, compWeaponDraw, pawn);
        }

        return false;
    }

    private static void MainHand(Thing eq, Vector3 drawLoc, float aimAngle, CompWeaponDraw comp, Pawn pawn)
    {
        var isStand = false;
        var isNorth = false;
        var num = aimAngle - 90f;
        if (pawn.Rotation == Rot4.South)
        {
            drawLoc -= new Vector3(0f, 0f, -0.22f);
            aimAngle -= 143f;
            isStand = true;
        }

        if (pawn.Rotation == Rot4.North)
        {
            drawLoc -= new Vector3(0f, 0f, -0.22f);
            aimAngle -= 143f;
            isStand = true;
            isNorth = true;
        }

        Mesh mesh;
        if (aimAngle is > 20f and < 160f)
        {
            mesh = MeshPool.plane10;
            num += eq.def.equippedAngleOffset;
        }
        else if (aimAngle is > 200f and < 340f)
        {
            mesh = MeshPool.plane10Flip;
            num -= 180f;
            num -= eq.def.equippedAngleOffset;
        }
        else
        {
            mesh = MeshPool.plane10;
            num += eq.def.equippedAngleOffset;
        }

        num += CompWeaponDraw.DrawAngleOffSet(pawn.Rotation, comp.Props.rotOffset, isStand, isNorth);
        num %= 360f;
        var material = comp.Props.rotOffset.graphicData?.Graphic.MatSingle ?? eq.Graphic.MatSingle;
        var matrix = default(Matrix4x4);
        var s = new Vector3(comp.Props.rotOffset.graphicData?.drawSize.x ?? eq.def.graphicData.drawSize.x, 1f,
            comp.Props.rotOffset.graphicData?.drawSize.y ?? eq.def.graphicData.drawSize.y);
        if (CompWeaponDraw.IsInverse(pawn.Rotation, comp.Props.rotOffset, isStand, isNorth))
        {
            mesh = !(mesh == MeshPool.plane10) ? MeshPool.plane10 : MeshPool.plane10Flip;
        }

        var pos = drawLoc + CompWeaponDraw.DrawPosOffSet(pawn.Rotation, comp.Props.rotOffset, isStand, isNorth);
        matrix.SetTRS(pos, Quaternion.AngleAxis(num, Vector3.up), s);
        Graphics.DrawMesh(mesh, matrix, material, 0);
    }

    private static void OffHand(Thing eq, Vector3 drawLoc, float aimAngle, CompWeaponDraw comp, Pawn pawn)
    {
        var isStand = false;
        var isNorth = false;
        var num = aimAngle - 90f;
        if (pawn.Rotation == Rot4.South)
        {
            drawLoc -= new Vector3(0f, 0f, -0.22f);
            aimAngle -= 143f;
            isStand = true;
        }

        if (pawn.Rotation == Rot4.North)
        {
            drawLoc -= new Vector3(0f, 0f, -0.22f);
            aimAngle -= 143f;
            isStand = true;
            isNorth = true;
        }

        Mesh mesh;
        if (aimAngle is > 20f and < 160f)
        {
            mesh = MeshPool.plane10;
            num += eq.def.equippedAngleOffset;
        }
        else if (aimAngle is > 200f and < 340f)
        {
            mesh = MeshPool.plane10Flip;
            num -= 180f;
            num -= eq.def.equippedAngleOffset;
        }
        else
        {
            mesh = MeshPool.plane10;
            num += eq.def.equippedAngleOffset;
        }

        num += CompWeaponDraw.DrawAngleOffSet(pawn.Rotation, comp.Props.offHand, isStand, isNorth);
        num %= 360f;
        var material = comp.Props.offHand.graphicData?.Graphic.MatSingle ?? eq.Graphic.MatSingle;
        var matrix = default(Matrix4x4);
        var s = new Vector3(comp.Props.offHand.graphicData?.drawSize.x ?? eq.def.graphicData.drawSize.x, 1f,
            comp.Props.offHand.graphicData?.drawSize.y ?? eq.def.graphicData.drawSize.y);
        if (CompWeaponDraw.IsInverse(pawn.Rotation, comp.Props.offHand, isStand, isNorth))
        {
            mesh = !(mesh == MeshPool.plane10) ? MeshPool.plane10 : MeshPool.plane10Flip;
        }

        var pos = drawLoc + CompWeaponDraw.DrawPosOffSet(pawn.Rotation, comp.Props.offHand, isStand, isNorth);
        matrix.SetTRS(pos, Quaternion.AngleAxis(num, Vector3.up), s);
        Graphics.DrawMesh(mesh, matrix, material, 0);
    }
}