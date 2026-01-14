using System.Collections.Generic;
using Verse.AI;

namespace MParmorLibrary;

public class JobDriver_ReturnToPit : JobDriver
{
    protected Building_FabricationPit Target => job.targetA.Thing as Building_FabricationPit;

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        return pawn.Reserve(Target, job, 1, -1, null, errorOnFailed);
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        yield return Toils_Goto.GotoCell(Target.Position, PathEndMode.OnCell);
        yield return Toils_General.Wait(30);
        yield return new Toil
        {
            initAction = delegate
            {
                pawn.GetMParmorCore(out var core);
                core.GetOutOfMParmor();
            },
            defaultCompleteMode = ToilCompleteMode.Instant
        };
    }
}