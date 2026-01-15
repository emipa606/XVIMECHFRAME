using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace MParmorLibrary;

public class JobDriver_ReturnToCarrier : JobDriver
{
    protected Pawn Target => job.targetA.Thing as Pawn;

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        return pawn.Reserve(Target, job, 1, -1, null, errorOnFailed);
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
        yield return new Toil
        {
            initAction = delegate
            {
                if (pawn is not Drone drone)
                {
                    return;
                }

                (drone.origin as MParmorCore_Carrier)?.RecycleDrone(drone.ShieldClass.Shield /
                                                                    drone.ShieldClass.ShieldMax);
                drone.spawnTick = -1500;
            },
            defaultCompleteMode = ToilCompleteMode.Instant
        };
    }
}