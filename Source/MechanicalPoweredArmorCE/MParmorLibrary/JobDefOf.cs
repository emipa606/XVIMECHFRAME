using RimWorld;
using Verse;

namespace MParmorLibrary;

[DefOf]
public static class JobDefOf
{
    public static JobDef XFMParmor_Job_GetIntoMParmor;

    public static JobDef XFMParmor_Job_FillChargingStation;

    public static JobDef XFMParmor_Job_TakeBattery;

    public static JobDef XFMPamor_DestructMParmor;

    public static JobDef XFMParmor_Job_ReturnToCarrier;

    public static JobDef XFMParmor_Job_ReturnToPit;

    static JobDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(JobDefOf));
    }
}