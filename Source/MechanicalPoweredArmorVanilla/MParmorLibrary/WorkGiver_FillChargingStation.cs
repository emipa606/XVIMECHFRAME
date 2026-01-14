using RimWorld;
using Verse;
using Verse.AI;

namespace MParmorLibrary;

public class WorkGiver_FillChargingStation : WorkGiver_Scanner
{
    public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForDef(ThingDefOf.XFMParmor_ChargingStation);

    public override PathEndMode PathEndMode => PathEndMode.Touch;

    public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        if (t is not Building_ChargingStation building_ChargingStation)
        {
            return false;
        }

        if (t.IsForbidden(pawn) || !pawn.CanReserve(t, 1, -1, null, forced))
        {
            return false;
        }

        if (pawn.Map.designationManager.DesignationOn(t, DesignationDefOf.Deconstruct) != null)
        {
            return false;
        }

        if (!building_ChargingStation.HasBattery && FindUnfilledBattrey(pawn) == null)
        {
            JobFailReason.Is("XFMParmor_WorkGiver_FillChargingStation_failReasonA".Translate());
            return false;
        }

        if (!building_ChargingStation.HasBattery || building_ChargingStation.IsBatteryFull)
        {
            return !t.IsBurning();
        }

        JobFailReason.Is("XFMParmor_WorkGiver_FillChargingStation_failReasonB".Translate());
        return false;
    }

    public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        var building_ChargingStation = (Building_ChargingStation)t;
        if (building_ChargingStation.HasBattery)
        {
            return JobMaker.MakeJob(JobDefOf.XFMParmor_Job_TakeBattery, t);
        }

        var thing = FindUnfilledBattrey(pawn);
        return JobMaker.MakeJob(JobDefOf.XFMParmor_Job_FillChargingStation, t, thing);
    }

    private static Thing FindUnfilledBattrey(Pawn pawn)
    {
        return GenClosest.ClosestThingReachable(pawn.Position, pawn.Map,
            ThingRequest.ForDef(ThingDefOf.XFMParmor_UnfilledPowerCell), PathEndMode.ClosestTouch,
            TraverseParms.For(pawn), 9999f, validator);

        bool validator(Thing x)
        {
            return !x.IsForbidden(pawn) && pawn.CanReserve(x);
        }
    }
}