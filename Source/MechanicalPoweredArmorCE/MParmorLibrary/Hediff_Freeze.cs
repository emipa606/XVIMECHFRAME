using System.Linq;
using RimWorld;
using Verse;

namespace MParmorLibrary;

public class Hediff_Freeze : HediffWithComps
{
    public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
    {
        base.Notify_PawnDied(dinfo, culprit);
        DoFlecks();
        Spread();
    }

    private void DoFlecks()
    {
        var loc = pawn.Corpse?.DrawPos ?? pawn.DrawPos;
        var map = pawn.Corpse == null ? pawn.Map : pawn.Corpse.Map;
        for (var i = 0; i < 6; i++)
        {
            FleckMaker.Static(loc, map, FleckDefOf.XFMParmor_IceCloud);
        }
    }

    private void Spread()
    {
        foreach (var item in (pawn.Corpse == null ? pawn.Map : pawn.Corpse.Map).mapPawns.AllPawnsSpawned)
        {
            if (item.Position.InHorDistOf(pawn.Corpse?.Position ?? pawn.Position, 1.5f) &&
                !pawn.Faction.HostileTo(item.Faction))
            {
                HealthUtility.AdjustSeverity(item, def, 1f);
            }
        }
    }

    public override void Notify_PawnPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
    {
        base.Notify_PawnPostApplyDamage(dinfo, totalDamageDealt);
        if (dinfo.Def.harmsHealth)
        {
            Affect(totalDamageDealt, ToolsLibrary.GetOutsideBodyPart(dinfo.HitPart));
        }
    }

    private void Affect(float amount, BodyPartRecord hitPart)
    {
        var num = AmountWork(amount) * 0.5f;
        if (Rand.Range(1, 3) == 2)
        {
            AddInjury(num * 0.6f, hitPart);
            AddInjury(num * 0.4f, hitPart);
        }
        else
        {
            AddInjury(num, hitPart);
        }
    }

    private void AddInjury(float amount, BodyPartRecord hitPart)
    {
        var list = (from x in pawn.health.hediffSet.GetNotMissingParts()
            where x.coverageAbs > 0f
            select x).ToList();
        if (list.NullOrEmpty())
        {
            return;
        }

        if (!list.Contains(hitPart) || hitPart.coverageAbs <= 0f)
        {
            hitPart = null;
        }

        hitPart ??= pawn.health.hediffSet.GetRandomNotMissingPart(RimWorld.DamageDefOf.Scratch,
            BodyPartHeight.Undefined, BodyPartDepth.Outside);

        var hediff_Injury = (Hediff_Injury)HediffMaker.MakeHediff(HediffDefOf.XFMParmor_Frostbite, pawn, hitPart);
        hediff_Injury.Severity = amount;
        pawn.health.AddHediff(hediff_Injury);
    }

    private static float AmountWork(float amount)
    {
        if (amount < 5f)
        {
            return amount;
        }

        return 5f + ((amount - 5f) * 0.5f);
    }
}