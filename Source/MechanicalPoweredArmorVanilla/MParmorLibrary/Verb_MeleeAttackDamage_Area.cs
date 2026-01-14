using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace MParmorLibrary;

public class Verb_MeleeAttackDamage_Area : Verb_MeleeAttackDamage
{
    protected override bool TryCastShot()
    {
        var casterPawn = CasterPawn;
        if (casterPawn.stances.FullBodyBusy)
        {
            return false;
        }

        var map = currentTarget.Thing.Map;
        var cell = currentTarget.Cell;
        var isPawn = currentTarget.Thing is Pawn;

        var result = base.TryCastShot();
        var method =
            typeof(Verb_MeleeAttack).GetMethod("CreateCombatLog", BindingFlags.Instance | BindingFlags.NonPublic);
        var func = (ManeuverDef maneuverDef) => maneuverDef.combatLogRulesHit;
        var method2 = typeof(Verb_MeleeAttack)
            .GetMethod("GetNonMissChance", BindingFlags.Instance | BindingFlags.NonPublic);
        var method3 = typeof(Verb_MeleeAttack)
            .GetMethod("SoundHitPawn", BindingFlags.Instance | BindingFlags.NonPublic);
        var method4 = typeof(Verb_MeleeAttack)
            .GetMethod("SoundDodge", BindingFlags.Instance | BindingFlags.NonPublic);
        var method5 = typeof(Verb_MeleeAttack)
            .GetMethod("SoundMiss", BindingFlags.Instance | BindingFlags.NonPublic);
        if (!isPawn)
        {
            return result;
        }

        foreach (var target in GetTargets(cell, map))
        {
            if (target is { Dead: false } &&
                (casterPawn.MentalStateDef != MentalStateDefOf.SocialFighting ||
                 target.MentalStateDef != MentalStateDefOf.SocialFighting) && (casterPawn.story == null ||
                                                                               !casterPawn.story.traits
                                                                                   .DisableHostilityFrom(target)))
            {
                target.mindState.meleeThreat = casterPawn;
                target.mindState.lastMeleeThreatHarmTick = Find.TickManager.TicksGame;
            }

            var drawPos = target.DrawPos;
            SoundDef soundDef;
            if (Rand.Chance((float)method2.Invoke(this, [target])))
            {
                if (!Rand.Chance((float)method2.Invoke(this, [target])))
                {
                    soundDef = method3.Invoke(this, []) as SoundDef;
                    if (verbProps.impactMote != null)
                    {
                        MoteMaker.MakeStaticMote(drawPos, map, verbProps.impactMote);
                    }

                    if (verbProps.impactFleck != null)
                    {
                        FleckMaker.Static(drawPos, map, verbProps.impactFleck);
                    }

                    var battleLogEntry_MeleeCombat =
                        method.Invoke(this, [func, true, target]) as BattleLogEntry_MeleeCombat;
                    result = true;
                    var damageResult = ApplyMeleeDamageToTarget(target);
                    if (damageResult.stunned && damageResult.parts.NullOrEmpty())
                    {
                        Find.BattleLog.RemoveEntry(battleLogEntry_MeleeCombat);
                    }
                    else
                    {
                        damageResult.AssociateWithLog(battleLogEntry_MeleeCombat);
                        if (damageResult.deflected)
                        {
                            battleLogEntry_MeleeCombat.RuleDef = maneuver.combatLogRulesDeflect;
                            battleLogEntry_MeleeCombat.alwaysShowInCompact = false;
                        }
                    }
                }
                else
                {
                    soundDef = method4.Invoke(this, [target]) as SoundDef;
                    MoteMaker.ThrowText(drawPos, map, "TextMote_Dodge".Translate(), 1.9f);
                    method.Invoke(this, [func, true, target]);
                }
            }
            else
            {
                soundDef = method5.Invoke(this, []) as SoundDef;
                method.Invoke(this, [func, true, target]);
            }

            soundDef.PlayOneShot(new TargetInfo(target.Position, map));
            if (target is { Dead: false, Spawned: true })
            {
                target.stances.stagger.StaggerFor(95);
            }
        }

        return result;
    }

    public virtual List<Pawn> GetTargets(IntVec3 Intv3, Map map)
    {
        return map.mapPawns.AllPawnsSpawned.Where(x => IsExtraTargets(x, Intv3)).ToList();
    }

    public virtual bool IsExtraTargets(Pawn pawn, IntVec3 Intv3)
    {
        return pawn != currentTarget.Thing && ToolsLibrary_MParmorOnly.IsUnfriendly(pawn, CasterPawn) &&
               pawn.Position.InHorDistOf(Intv3, 1.5f) && !pawn.Downed;
    }

    public override void DrawHighlight(LocalTargetInfo target)
    {
        base.DrawHighlight(target);
        if (!target.IsValid)
        {
            return;
        }

        GenDraw.DrawFieldEdges(GetField(target.Cell), new Color(0.30980393f, 83f / 85f, 0.9372549f, 0.75f));
        foreach (var target2 in GetTargets(target.Cell, target.Thing.Map))
        {
            GenDraw.DrawTargetHighlight(new LocalTargetInfo(target2));
        }
    }

    private static List<IntVec3> GetField(IntVec3 center)
    {
        return
        [
            center,
            center + new IntVec3(1, 0, 0),
            center + new IntVec3(-1, 0, 0),
            center + new IntVec3(0, 0, 1),
            center + new IntVec3(0, 0, -1),
            center + new IntVec3(1, 0, 1),
            center + new IntVec3(1, 0, -1),
            center + new IntVec3(-1, 0, 1),
            center + new IntVec3(-1, 0, -1)
        ];
    }
}