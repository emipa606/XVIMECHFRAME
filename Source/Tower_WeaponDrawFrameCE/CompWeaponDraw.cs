using System;
using UnityEngine;
using Verse;

namespace Tower_WeaponDrawFrame;

public class CompWeaponDraw : ThingComp
{
    public Func<bool> showMainHandNow;
    public Func<bool> showOffHandNow;

    public CompProperties_WeaponDraw Props => props as CompProperties_WeaponDraw;

    public static bool IsInverse(Rot4 facing, RotOffset offset, bool isStand = false, bool isNorth = false)
    {
        if (isStand)
        {
            return isNorth ? offset.northStand.inverse : offset.stand.inverse;
        }

        if (facing == Rot4.North)
        {
            return offset.north.inverse;
        }

        if (facing == Rot4.East)
        {
            return offset.east.inverse;
        }

        if (facing == Rot4.South)
        {
            return offset.south.inverse;
        }

        return facing == Rot4.West && offset.west.inverse;
    }

    public static Vector3 DrawPosOffSet(Rot4 facing, RotOffset offset, bool isStand = false, bool isNorth = false)
    {
        if (isStand)
        {
            return isNorth ? offset.northStand.offset : offset.stand.offset;
        }

        if (facing == Rot4.North)
        {
            return offset.north.offset;
        }

        if (facing == Rot4.East)
        {
            return offset.east.offset;
        }

        if (facing == Rot4.South)
        {
            return offset.south.offset;
        }

        return facing == Rot4.West ? offset.west.offset : new Vector3(0f, 0f, 0f);
    }

    public static float DrawAngleOffSet(Rot4 facing, RotOffset offset, bool isStand = false, bool isNorth = false)
    {
        if (isStand)
        {
            return isNorth ? offset.northStand.angle : offset.stand.angle;
        }

        if (facing == Rot4.North)
        {
            return offset.north.angle;
        }

        if (facing == Rot4.East)
        {
            return offset.east.angle;
        }

        if (facing == Rot4.South)
        {
            return offset.south.angle;
        }

        return facing == Rot4.West ? offset.west.angle : 0f;
    }
}