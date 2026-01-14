using RimWorld;
using Verse;

namespace MParmorLibrary;

public class JobGiver_AIFightEnemies_Drone : JobGiver_AIFightEnemies
{
    protected override void UpdateEnemyTarget(Pawn pawn)
    {
        if (pawn is Drone { searthNewTarget: false })
        {
            var enemyTarget = pawn.mindState.enemyTarget;
            base.UpdateEnemyTarget(pawn);
            if (pawn.mindState.enemyTarget != enemyTarget)
            {
                pawn.mindState.enemyTarget = null;
            }
        }
        else
        {
            base.UpdateEnemyTarget(pawn);
        }
    }
}