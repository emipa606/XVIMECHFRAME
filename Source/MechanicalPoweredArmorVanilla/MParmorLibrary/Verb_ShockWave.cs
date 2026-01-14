using System.Collections.Generic;
using MParmorLibrary.SkillSystem;
using RimWorld;
using UnityEngine;
using Verse;

namespace MParmorLibrary;

public class Verb_ShockWave : Verb
{
    protected override bool TryCastShot()
    {
        if (CasterPawn == null)
        {
            return false;
        }

        StartSkill();
        return true;
    }

    public void StartSkill()
    {
        var skillObject = DirectOwner as SkillObject;
        skillObject?.UsedOnce();
        var list = new List<Pawn>();
        foreach (var item in CasterPawn.Map.mapPawns.AllPawnsSpawned)
        {
            if (item.Position.InHorDistOf(CasterPawn.Position, EffectiveRange) &&
                ToolsLibrary_MParmorOnly.IsUnfriendly(CasterPawn, item))
            {
                list.Add(item);
            }
        }

        if (list.Count > 0)
        {
            list[^1].TakeDamage(new DamageInfo(RimWorld.DamageDefOf.EMP, RimWorld.DamageDefOf.EMP.defaultDamage, 0f,
                -1f, CasterPawn, null, skillObject?.parent.parent.def));
            list.RemoveAt(list.Count - 1);
        }

        GenExplosion.DoExplosion(CasterPawn.Position, CasterPawn.Map, EffectiveRange,
            DamageDefOf.MParmor_Black_BombDamage, CasterPawn, 50, 0f, RimWorld.DamageDefOf.Bomb.soundExplosion, null,
            null, null, null, 0f, 1, null, null, 0);
        FleckMaker.AttachedOverlay(Caster, FleckDefOf.XFMParmor_Fleck_ShockWave, new Vector3(0f, 0f, 0f));
    }

    public override void DrawHighlight(LocalTargetInfo target)
    {
        GenDraw.DrawRadiusRing(CasterPawn.Position, EffectiveRange,
            new Color(0.30980393f, 83f / 85f, 0.9372549f, 0.75f), c => CanHitTarget(new LocalTargetInfo(c)));
    }
}