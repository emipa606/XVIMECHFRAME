using Verse;

namespace MParmorLibrary;

public class DamageWorker_ShockWave : DamageWorker
{
    public override DamageResult Apply(DamageInfo dinfo, Thing victim)
    {
        if (!ToolsLibrary_MParmorOnly.IsUnfriendly(dinfo, victim))
        {
            return new DamageResult();
        }

        if (victim is not Pawn pawn)
        {
            return base.Apply(dinfo, victim);
        }

        var result = base.Apply(dinfo, victim);
        pawn.stances.stunner.StunFor(300, dinfo.Instigator);
        pawn.stances.stagger.StaggerFor(60);
        return result;
    }
}