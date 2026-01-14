using RimWorld;
using Verse;

namespace MParmorLibrary;

[DefOf]
public static class HediffDefOf
{
    public static HediffDef XFMParmor_Frostbite;

    public static HediffDef XFMParmor_Freeze;

    public static HediffDef XFMParmor_ArcticWind;

    public static HediffDef XFMParmor_MissingBodyPart_NotInstalled;

    public static HediffDef XFMParmor_MissingBodyPart_NotInstalled_Blade;

    public static HediffDef XFMParmor_Weariness;

    static HediffDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(HediffDefOf));
    }
}