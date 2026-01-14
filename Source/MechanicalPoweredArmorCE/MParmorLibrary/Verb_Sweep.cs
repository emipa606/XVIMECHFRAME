using System;
using System.Collections.Generic;
using System.Linq;
using MParmorLibrary.SkillSystem;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace MParmorLibrary;

public class Verb_Sweep : Verb
{
    public readonly List<IntVec3> resultCache = [];

    public float angleCache;
    public IntVec3 casterCache;

    public float radiusCache;

    public Vector3 targetCache;

    public SkillObject Skill => DirectOwner as SkillObject;

    public override float EffectiveRange => verbProps.range;

    public override bool MultiSelect => true;

    public override bool ValidateTarget(LocalTargetInfo target, bool showMessages = true)
    {
        return caster != null && CanHitTarget(target) && JumpUtility.ValidJumpTarget(caster, caster.Map, target.Cell);
    }

    public override bool CanHitTargetFrom(IntVec3 root, LocalTargetInfo targ)
    {
        var num = EffectiveRange * EffectiveRange;
        var cell = targ.Cell;
        return caster.Position.DistanceToSquared(cell) <= num && GenSight.LineOfSight(root, cell, caster.Map, false);
    }

    public override void OnGUI(LocalTargetInfo target)
    {
        if (CanHitTarget(target) && JumpUtility.ValidJumpTarget(caster, caster.Map, target.Cell))
        {
            base.OnGUI(target);
        }
        else
        {
            GenUI.DrawMouseAttachment(TexCommand.CannotShoot);
        }
    }

    public override void DrawHighlight(LocalTargetInfo target)
    {
        if (target.IsValid && JumpUtility.ValidJumpTarget(caster, caster.Map, target.Cell))
        {
            GenDraw.DrawTargetHighlightWithLayer(target.CenterVector3, AltitudeLayer.Silhouettes);
        }

        GenDraw.DrawRadiusRing(caster.Position, EffectiveRange, Color.white,
            c => GenSight.LineOfSight(caster.Position, c, caster.Map, false) &&
                 JumpUtility.ValidJumpTarget(caster, caster.Map, c));
        GenDraw.DrawFieldEdges(
            GetSectorCells(target.CenterVector3, EffectiveRange, Skill.skill.skillValueA,
                c => GenSight.LineOfSight(caster.Position, c, Caster.Map)),
            new Color(0.30980393f, 83f / 85f, 0.9372549f, 0.75f));
        foreach (var target2 in GetTargets(target.Cell))
        {
            GenDraw.DrawTargetHighlight(new LocalTargetInfo(target2));
        }
    }

    public override void OrderForceTarget(LocalTargetInfo target)
    {
        var job = JobMaker.MakeJob(RimWorld.JobDefOf.CastJump, target);
        job.verbToUse = this;
        CasterPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
    }

    protected override bool TryCastShot()
    {
        var casterPawn = CasterPawn;
        var cell = currentTarget.Cell;
        var map = casterPawn.Map;
        var targets = GetTargets(cell);
        targets.SortBy(x => 0f - ToolsLibrary.GetDistance(x.Position, caster.Position));
        Skill.UsedOnce();
        Skill.ChargeEnergy(((MParmorCore_Red)Skill.Core).attactSpeedLevel * 60);
        var pawnSweeper = PawnSweeper.MakeSweeper(casterPawn, casterPawn.DrawPos, targets);
        if (pawnSweeper == null)
        {
            return false;
        }

        GenSpawn.Spawn(pawnSweeper, cell, map);
        return true;
    }

    public List<IntVec3> GetSectorCells(Vector3 target, float radius, float angle, Func<IntVec3, bool> predicate = null)
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
            if (ToolsLibrary.AngleAlgorithm((item2.ToVector3Shifted() - Caster.TrueCenter()).AngleFlat(), angleB,
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

    public virtual List<Pawn> GetTargets(IntVec3 intVec3)
    {
        return Caster.Map.mapPawns.AllPawnsSpawned.Where(x => IsTargets(x, intVec3)).ToList();
    }

    public virtual bool IsTargets(Pawn pawn, IntVec3 intVec3)
    {
        return ToolsLibrary_MParmorOnly.IsUnfriendly(pawn, CasterPawn) &&
               ToolsLibrary.IsInSector(caster.DrawPos, pawn.DrawPos, EffectiveRange,
                   (intVec3.ToVector3Shifted() - caster.DrawPos).AngleFlat(), Skill.skill.skillValueA) &&
               !pawn.Downed && GenSight.LineOfSight(caster.Position, pawn.Position, Caster.Map);
    }
}