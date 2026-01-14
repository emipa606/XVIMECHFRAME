using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace MParmorLibrary;

public class PawnFlyer_RedSkill : PawnFlyer, IAttackTarget
{
    public static readonly float radius = 3.9f;

    Thing IAttackTarget.Thing => this;

    LocalTargetInfo IAttackTarget.TargetCurrentlyAimingAt => LocalTargetInfo.Invalid;

    float IAttackTarget.TargetPriorityFactor => 1f;

    bool IAttackTarget.ThreatDisabled(IAttackTargetSearcher disabledFor)
    {
        return !Spawned;
    }

    protected override void DrawAt(Vector3 drawLoc, bool flip = false)
    {
        base.DrawAt(drawLoc, flip);
        if (FlyingPawn.apparel == null)
        {
            return;
        }

        var wornApparel = FlyingPawn.apparel.WornApparel;
        foreach (var item in wornApparel)
        {
            if (item is MParmorCore mParmorCore)
            {
                mParmorCore.MParmorDrawAt(DrawPos);
            }
        }
    }

    protected override void RespawnPawn()
    {
        GenExplosion.DoExplosion(Position, Map, radius, RimWorld.DamageDefOf.Crush, FlyingPawn, 20, 70f,
            RimWorld.DamageDefOf.Bomb.soundExplosion, ThingDefOf.XFMParmor_MechanicalArmorCore_Red, null, null, null,
            0f, 1, null, null, 0);
        base.RespawnPawn();
    }
}