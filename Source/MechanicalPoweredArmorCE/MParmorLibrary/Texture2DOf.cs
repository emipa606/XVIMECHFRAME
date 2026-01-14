using UnityEngine;
using Verse;

namespace MParmorLibrary;

[StaticConstructorOnStartup]
public static class Texture2DOf
{
    public static readonly Texture2D GetOut = ContentFinder<Texture2D>.Get("XFMParmor/GetOut");

    public static readonly Texture2D SetTargetFuelLevelCommand =
        ContentFinder<Texture2D>.Get("UI/Commands/SetTargetFuelLevel");

    public static readonly Texture2D RedAttactSpeed = ContentFinder<Texture2D>.Get("XFMParmor/MParmor/Red/AttackSpeed");

    public static readonly Texture2D DroneIcon = ContentFinder<Texture2D>.Get("XFMParmor/MParmor/Carrier/DroneIcon");

    public static readonly Texture2D AiFight = ContentFinder<Texture2D>.Get("XFMParmor/AiFight");

    public static readonly Texture2D Attact = ContentFinder<Texture2D>.Get("UI/Commands/Attack");
}