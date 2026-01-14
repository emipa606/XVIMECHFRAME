using RimWorld;
using Verse;
using Verse.AI;

namespace MParmorLibrary;

public class JobGiver_AIFightEnemiesFollowSecocnd : JobGiver_AIFightEnemies
{
    public float distance;

    public override ThinkNode DeepCopy(bool resolve = true)
    {
        var jobGiver_AIFightEnemiesFollowSecocnd = (JobGiver_AIFightEnemiesFollowSecocnd)base.DeepCopy(resolve);
        jobGiver_AIFightEnemiesFollowSecocnd.distance = distance;
        return jobGiver_AIFightEnemiesFollowSecocnd;
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
                validator = cell => (pawn.mindState.duty.focusSecond.Pawn.Position - cell).LengthHorizontalSquared <
                                    distance * distance
            }, out dest);
        }

        dest = IntVec3.Invalid;
        return false;
    }
}