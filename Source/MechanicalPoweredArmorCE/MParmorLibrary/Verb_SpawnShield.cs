using System.Collections.Generic;
using System.Linq;
using MParmorLibrary.SkillSystem;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace MParmorLibrary;

public class Verb_SpawnShield : Verb
{
    public MechanicalArmorDef MPArmor =>
        (DirectOwner as SkillObject)?.parent.parent.GetComp<CompMechanicalArmor>().MPArmor;

    public override void DrawHighlight(LocalTargetInfo target)
    {
        base.DrawHighlight(target);
        var angle = (target.CenterVector3 - Caster.TrueCenter()).AngleFlat();
        var rotaion = ToolsLibrary.RotFromAngle(angle);
        GenDraw.DrawFieldEdges(GetShieldCells(rotaion, target.Cell),
            new Color(0.30980393f, 83f / 85f, 0.9372549f, 0.75f));
    }

    public static List<IntVec3> GetShieldCells(Rot4 rotaion, IntVec3 center)
    {
        if (rotaion == Rot4.East)
        {
            return ToolsLibrary.GetCellsInRect(center + new IntVec3(0, 0, 3), center + new IntVec3(1, 0, -3));
        }

        if (rotaion == Rot4.West)
        {
            return ToolsLibrary.GetCellsInRect(center + new IntVec3(0, 0, 3), center + new IntVec3(-1, 0, -3));
        }

        if (rotaion == Rot4.North)
        {
            return ToolsLibrary.GetCellsInRect(center + new IntVec3(-3, 0, 0), center + new IntVec3(3, 0, 1));
        }

        return rotaion == Rot4.South
            ? ToolsLibrary.GetCellsInRect(center + new IntVec3(-3, 0, 0), center + new IntVec3(3, 0, -1))
            : Enumerable.Empty<IntVec3>().ToList();
    }

    protected override bool TryCastShot()
    {
        var casterPawn = CasterPawn;
        if (casterPawn == null)
        {
            return false;
        }

        if (DirectOwner is not SkillObject skillObject)
        {
            return false;
        }

        if (skillObject.thingHolder is ShieldObstacle { Spawned: not false } shieldObstacle)
        {
            shieldObstacle.SheildBreak();
        }

        skillObject.UsedOnce();
        var angle = (currentTarget.CenterVector3 - Caster.TrueCenter()).AngleFlat();
        var rot = ToolsLibrary.RotFromAngle(angle);
        var shieldObstacle2 = ThingMaker.MakeThing(ThingDefOf.XFMParmor_Black_Shield) as ShieldObstacle;
        if (skillObject.skill.skillValueA != -1f)
        {
            shieldObstacle2?.ShieldClass.ShieldMax = skillObject.skill.skillValueA;
        }

        shieldObstacle2?.holder = skillObject;
        skillObject.thingHolder = shieldObstacle2;
        shieldObstacle2?.SetFaction(CasterPawn.Faction);
        GenSpawn.Spawn(shieldObstacle2, currentTarget.Cell, CasterPawn.Map, rot);
        return true;
    }

    public override void OrderForceTarget(LocalTargetInfo target)
    {
        var job = JobMaker.MakeJob(RimWorld.JobDefOf.UseVerbOnThing, target);
        job.verbToUse = this;
        CasterPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
    }
}