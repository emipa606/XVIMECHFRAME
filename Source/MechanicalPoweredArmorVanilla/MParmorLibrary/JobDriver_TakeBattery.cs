using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace MParmorLibrary;

public class JobDriver_TakeBattery : JobDriver
{
    protected Building_ChargingStation ChargingStation => (Building_ChargingStation)job.GetTarget(TargetIndex.A).Thing;

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        return pawn.Reserve(ChargingStation, job, 1, -1, null, errorOnFailed);
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
        this.FailOnBurningImmobile(TargetIndex.A);
        yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
        yield return Toils_General.Wait(200).FailOnDestroyedNullOrForbidden(TargetIndex.A)
            .FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch)
            .FailOn(() => !ChargingStation.IsBatteryFull)
            .WithProgressBarToilDelay(TargetIndex.A);
        yield return new Toil
        {
            initAction = delegate
            {
                var thing = ChargingStation.TakeBattery();
                GenPlace.TryPlaceThing(thing, pawn.Position, Map, ThingPlaceMode.Near, null, null, default(Rot4));
                var currentPriority = StoreUtility.CurrentStoragePriorityOf(thing);
                if (StoreUtility.TryFindBestBetterStoreCellFor(thing, pawn, Map, currentPriority, pawn.Faction,
                        out var foundCell))
                {
                    job.SetTarget(TargetIndex.C, foundCell);
                    job.SetTarget(TargetIndex.B, thing);
                    job.count = thing.stackCount;
                }
                else
                {
                    EndJobWith(JobCondition.Incompletable);
                }
            },
            defaultCompleteMode = ToilCompleteMode.Instant
        };
        yield return Toils_Reserve.Reserve(TargetIndex.B);
        yield return Toils_Reserve.Reserve(TargetIndex.C);
        yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.ClosestTouch);
        yield return Toils_Haul.StartCarryThing(TargetIndex.B);
        var carryToCell = Toils_Haul.CarryHauledThingToCell(TargetIndex.C);
        yield return carryToCell;
        yield return Toils_Haul.PlaceHauledThingInCell(TargetIndex.C, carryToCell, true);
    }
}