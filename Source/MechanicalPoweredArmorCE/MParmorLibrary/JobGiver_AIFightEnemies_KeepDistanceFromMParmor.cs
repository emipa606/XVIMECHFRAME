using RimWorld;
using Verse;
using Verse.AI;

namespace MParmorLibrary;

public class JobGiver_AIFightEnemies_KeepDistanceFromMParmor : JobGiver_AIFightEnemies
{
    public float distance;

    public override ThinkNode DeepCopy(bool resolve = true)
    {
        var jobGiver_AIFightEnemies_KeepDistanceFromMParmor =
            (JobGiver_AIFightEnemies_KeepDistanceFromMParmor)base.DeepCopy(resolve);
        jobGiver_AIFightEnemies_KeepDistanceFromMParmor.distance = distance;
        return jobGiver_AIFightEnemies_KeepDistanceFromMParmor;
    }

    protected override bool TryFindShootingPosition(Pawn pawn, out IntVec3 dest, Verb verbToUse = null)
    {
        var enemyTarget = pawn.mindState.enemyTarget;
        var allowManualCastWeapons = !pawn.IsColonist;
        var verb = pawn.TryGetAttackVerb(enemyTarget, allowManualCastWeapons);
        if (verb != null)
        {
            return CastPositionFinder.TryFindCastPosition(new CastPositionRequest
            {
                caster = pawn,
                target = enemyTarget,
                verb = verb,
                maxRangeFromTarget = verb.verbProps.range,
                wantCoverFromTarget = verb.verbProps.range > 5f,
                validator = CanBeAPos
            }, out dest);
        }

        dest = IntVec3.Invalid;
        return false;

        bool CanBeAPos(IntVec3 c)
        {
            foreach (var item in ToolsLibrary_MParmorOnly.GetMParmor(pawn.Map))
            {
                if ((item.PositionFixed - c).LengthHorizontalSquared < distance * distance)
                {
                    return false;
                }
            }

            return true;
        }
    }
}