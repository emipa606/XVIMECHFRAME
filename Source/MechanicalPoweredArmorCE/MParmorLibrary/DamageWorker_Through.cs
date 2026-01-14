using Verse;

namespace MParmorLibrary;

public class DamageWorker_Through : DamageWorker_AddInjury
{
    public override DamageResult Apply(DamageInfo dinfo, Thing thing)
    {
        var damageResult = base.Apply(dinfo, thing);
        if (thing is Pawn pawn && dinfo.HitPart == pawn.health.hediffSet.GetBrain())
        {
            damageResult.headshot = true;
        }

        return damageResult;
    }

    protected override void ApplySpecialEffectsToPart(Pawn pawn, float totalDamage, DamageInfo dinfo,
        DamageResult result)
    {
        var outsideBodyParts = ToolsLibrary.GetOutsideBodyParts(dinfo.HitPart);
        var num = 1.5f / outsideBodyParts.Count > 1f ? 1f : 1.5f / outsideBodyParts.Count;
        totalDamage *= num;
        for (var num2 = outsideBodyParts.Count; num2 > 0; num2--)
        {
            var dinfo2 = new DamageInfo(dinfo);
            dinfo2.SetHitPart(outsideBodyParts[num2 - 1]);
            FinalizeAndAddInjury(pawn, totalDamage, dinfo2, result);
            totalDamage *= 0.75f;
        }
    }
}