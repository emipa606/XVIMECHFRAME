using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace MParmorLibrary;

public class ThinkNode_SpawnShield : ThinkNode
{
    public override ThinkResult TryIssueJobPackage(Pawn pawn, JobIssueParams jobParams)
    {
        ThingWithComps thingWithComps = null;
        foreach (var item in pawn.apparel.WornApparel)
        {
            if (item.def.defName != "XFMParmor_AntiMParmor_ShieldSpawner")
            {
                continue;
            }

            thingWithComps = item;
            break;
        }

        if (thingWithComps == null ||
            pawn.GetLord()?.CurLordToil is not LordToil_AntiMParmorFight lordToil_AntiMParmorFight ||
            lordToil_AntiMParmorFight.Data.shield != null &&
            Find.TickManager.TicksGame - lordToil_AntiMParmorFight.Data.lastSpawnShieldTime <= 420)
        {
            return ThinkResult.NoJob;
        }

        var num = 0;
        foreach (var item2 in lordToil_AntiMParmorFight.Data.protectedTarget)
        {
            if (!HarmedRecently(item2))
            {
                continue;
            }

            num++;
            if (num <= 1)
            {
                continue;
            }

            (thingWithComps.GetComp<CompApparelVerbOwner_Charged>().AllVerbs[0] as Verb_SpawnShield_Lord)
                ?.UseOnce();
            return ThinkResult.NoJob;
        }

        return ThinkResult.NoJob;
    }

    private static bool HarmedRecently(Pawn pawn)
    {
        return Find.TickManager.TicksGame - pawn.mindState.lastHarmTick < 540;
    }
}