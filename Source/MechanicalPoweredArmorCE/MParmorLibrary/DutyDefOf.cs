using RimWorld;
using Verse.AI;

namespace MParmorLibrary;

[DefOf]
public static class DutyDefOf
{
    public static DutyDef XFMParmor_Destructer;

    public static DutyDef XFMParmor_DestructerFollower;

    public static DutyDef XFMParmor_AntiMParmor_Melee;

    public static DutyDef XFMParmor_AntiMParmor_LongDistance;

    public static DutyDef XFMParmor_AntiMParmor_Leader;

    public static DutyDef XFMParmor_AntiMParmor_Rocket;

    public static DutyDef XFMParmor_AntiMParmor_Standard;

    static DutyDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(DutyDefOf));
    }
}