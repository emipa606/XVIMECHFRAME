using System.Collections.Generic;
using Verse.AI;

namespace MParmorLibrary;

public class JobDriver_FillChargingStation : JobDriver
{
    protected Building_ChargingStation ChargingStation => (Building_ChargingStation)job.GetTarget(TargetIndex.A).Thing;

    protected PowerCell Battery => job.GetTarget(TargetIndex.B).Thing as PowerCell;

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        return pawn.Reserve(ChargingStation, job, 1, -1, null, errorOnFailed) &&
               pawn.Reserve(Battery, job, 1, -1, null, errorOnFailed);
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
        this.FailOnBurningImmobile(TargetIndex.A);
        AddEndCondition(() => !ChargingStation.HasBattery ? JobCondition.Ongoing : JobCondition.Succeeded);
        yield return Toils_General.DoAtomic(delegate { job.count = 1; });
        var reserveWort = Toils_Reserve.Reserve(TargetIndex.B);
        yield return reserveWort;
        yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.ClosestTouch)
            .FailOnDespawnedNullOrForbidden(TargetIndex.B).FailOnSomeonePhysicallyInteracting(TargetIndex.B);
        yield return Toils_Haul.StartCarryThing(TargetIndex.B, false, true)
            .FailOnDestroyedNullOrForbidden(TargetIndex.B);
        yield return Toils_Haul.CheckForGetOpportunityDuplicate(reserveWort, TargetIndex.B, TargetIndex.None, true);
        yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
        yield return Toils_General.Wait(200).FailOnDestroyedNullOrForbidden(TargetIndex.B)
            .FailOnDestroyedNullOrForbidden(TargetIndex.A)
            .FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch)
            .WithProgressBarToilDelay(TargetIndex.A);
        yield return new Toil
        {
            initAction = delegate { ChargingStation.FillBattery(Battery); },
            defaultCompleteMode = ToilCompleteMode.Instant
        };
    }
}