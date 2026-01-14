using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace MParmorLibrary;

public class Verb_SpawnShield_Lord : Verb
{
    protected override bool TryCastShot()
    {
        UseOnce();
        return true;
    }

    public void UseOnce()
    {
        var shieldObstacle = ThingMaker.MakeThing(ThingDefOf.XFMParmor_AntiMParmor_Shield) as ShieldObstacle;
        shieldObstacle?.shieldClass.ShieldMax = 450f;
        shieldObstacle?.SetFaction(CasterPawn.Faction);
        GenSpawn.Spawn(shieldObstacle, Caster.Position, CasterPawn.Map);
        if (CasterPawn?.GetLord()?.CurLordToil is LordToil_AntiMParmorFight lordToil_AntiMParmorFight)
        {
            lordToil_AntiMParmorFight.Data.shield = shieldObstacle;
            lordToil_AntiMParmorFight.Data.lastSpawnShieldTime = Find.TickManager.TicksGame;
        }

        ReloadableCompSource?.UsedOnce();
    }

    public override void OrderForceTarget(LocalTargetInfo target)
    {
        var job = JobMaker.MakeJob(RimWorld.JobDefOf.UseVerbOnThing, target);
        job.verbToUse = this;
        CasterPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
    }
}