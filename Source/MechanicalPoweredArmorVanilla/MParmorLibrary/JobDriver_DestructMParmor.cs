using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace MParmorLibrary;

public class JobDriver_DestructMParmor : JobDriver
{
    public const int WorkerValue = 1200;

    private int totalNeededWork;

    private int workLeft;

    protected Thing Target => job.targetA.Thing;

    protected Building Building => (Building)Target.GetInnerIfMinified();

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        return Target.Spawned && pawn.Reserve(Target, job, 1, -1, null, errorOnFailed);
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
        var doWork = new Toil().FailOnDestroyedNullOrForbidden(TargetIndex.A)
            .FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
        doWork.initAction = delegate
        {
            totalNeededWork = 1200;
            workLeft = totalNeededWork;
        };
        doWork.tickAction = delegate
        {
            workLeft--;
            if (workLeft <= 0f)
            {
                doWork.actor.jobs.curDriver.ReadyForNextToil();
            }
        };
        doWork.defaultCompleteMode = ToilCompleteMode.Never;
        doWork.WithProgressBar(TargetIndex.A, () => 1f - (workLeft / (float)totalNeededWork));
        yield return doWork;
        yield return new Toil
        {
            initAction = delegate
            {
                Target.Faction?.Notify_BuildingRemoved(Building, pawn);

                Target.Destroy(DestroyMode.Deconstruct);
                pawn.records.Increment(RecordDefOf.ThingsDeconstructed);
                Map.designationManager.RemoveAllDesignationsOn(Target);
            },
            defaultCompleteMode = ToilCompleteMode.Instant
        };
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref workLeft, "workLeft");
        Scribe_Values.Look(ref totalNeededWork, "totalNeededWork");
    }
}