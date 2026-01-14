using RimWorld;

namespace MParmorLibrary;

[DefOf]
public static class IncidentDefOf
{
    public static IncidentDef MParmor_WreckageFall;

    public static IncidentDef XFMParmor_GiveQuest_SleepingMechanoids;

    public static IncidentDef XFMParmor_RaidEnemy_AntiMPArmor;

    static IncidentDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(IncidentDefOf));
    }
}