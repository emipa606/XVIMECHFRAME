using UnityEngine;
using Verse;

namespace MParmorLibrary;

[StaticConstructorOnStartup]
public static class MaterialOf
{
    public static readonly Material DriverShield =
        MaterialPool.MatFrom("XFMParmor/Shield", ShaderDatabase.TransparentPostLight);

    public static readonly Material DroneShield = MaterialPool.MatFrom("XFMParmor/Shield",
        ShaderDatabase.TransparentPostLight, new Color(1f, 1f, 1f, 0.2f));

    public static readonly Material Drone =
        MaterialPool.MatFrom("XFMParmor/MParmor/Carrier/Drone", ShaderDatabase.TransparentPostLight);

    public static readonly Material SolarPower_Bracket =
        MaterialPool.MatFrom("XFMParmor/Buildings/SolarPower_Bracket", ShaderDatabase.Cutout);

    public static readonly Material SolarPower_Plant =
        MaterialPool.MatFrom("XFMParmor/Buildings/SolarPower_Plant", ShaderDatabase.Cutout);
}