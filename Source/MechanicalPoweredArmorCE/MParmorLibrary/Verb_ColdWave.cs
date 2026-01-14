using System;
using System.Collections.Generic;
using MParmorLibrary.SkillSystem;
using RimWorld;
using UnityEngine;
using Verse;

namespace MParmorLibrary;

public class Verb_ColdWave : Verb_SectorDetermine
{
    public override void DrawHighlight(LocalTargetInfo target)
    {
        base.DrawHighlight(target);
        if (!(target != null))
        {
            return;
        }

        _ = target.CenterVector3;

        ThingDefOf.XFMParmor_Black_Shield.fillPercent = 1f;
        GenDraw.DrawFieldEdges(
            GetSectorCells(target.CenterVector3, EffectiveRange, Skill.skill.skillValueA,
                c => GenSight.LineOfSight(caster.Position, c, Caster.Map)),
            new Color(0.30980393f, 83f / 85f, 0.9372549f, 0.75f));
        ThingDefOf.XFMParmor_Black_Shield.fillPercent = 0f;
    }

    protected override bool TryCastShot()
    {
        var skillObject = DirectOwner as SkillObject;
        skillObject?.UsedOnce();
        DamageWorker_ExplosionWithDirection_Ice.direction =
            (CurrentTarget.CenterVector3 - Caster.TrueCenter()).AngleFlat();
        DamageWorker_ExplosionWithDirection_Ice.angle = Skill.skill.skillValueA;
        GenExplosion.DoExplosion(CasterPawn.Position, CasterPawn.Map, EffectiveRange, DamageDefOf.MParmor_Bomb_ColdWave,
            CasterPawn, 50, 0f, RimWorld.DamageDefOf.Bomb.soundExplosion, null, null, null, null, 0f, 1, null, null, 0);
        return true;
    }

    public override List<IntVec3> GetSectorCells(Vector3 target, float radius, float angle,
        Func<IntVec3, bool> predicate = null)
    {
        if (CasterPawn.Position == casterCache && target == targetCache && angleCache == angle && radiusCache == radius)
        {
            return resultCache;
        }

        var list = new List<IntVec3>();
        var list2 = new List<IntVec3>();
        var num = GenRadial.NumCellsInRadius(radius);
        for (var i = 0; i < num; i++)
        {
            var item = Caster.Position + GenRadial.RadialPattern[i];
            list.Add(item);
        }

        var angleB = (target - Caster.TrueCenter()).AngleFlat();
        foreach (var item2 in list)
        {
            if ((item2 - caster.Position).LengthHorizontalSquared < 6.25f)
            {
                list2.Add(item2);
            }
            else if (ToolsLibrary.AngleAlgorithm((item2.ToVector3Shifted() - Caster.TrueCenter()).AngleFlat(), angleB,
                         angle) && (predicate == null || predicate(item2)))
            {
                list2.Add(item2);
            }
        }

        casterCache = Caster.Position;
        targetCache = target;
        angleCache = angle;
        resultCache.Clear();
        resultCache.AddRange(resultCache);
        return list2;
    }
}