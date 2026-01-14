using MParmorLibrary.SkillSystem;
using Verse;
using Verse.AI;

namespace MParmorLibrary;

public class MParmorT_AITracker_Red : MParmorT_AITracker
{
    public override void Tick()
    {
        if (Find.TickManager.TicksGame % 10 == 0 && core.Wearer.CurJobDef == RimWorld.JobDefOf.Wait_Combat)
        {
            var targets = GenAiSystem.GetTargets(core.Wearer, core.Wearer.Map, 20f);
            if (targets.Count > 0)
            {
                targets.SortBy(x => ToolsLibrary.GetDistance(x.Position, core.Wearer.Position));
                var job = JobMaker.MakeJob(RimWorld.JobDefOf.AttackMelee, targets[0]);
                job.expiryInterval = new IntRange(360, 480).RandomInRange;
                job.checkOverrideOnExpire = true;
                core.Wearer.jobs.TryTakeOrderedJob(job, JobTag.DraftedOrder);
            }
        }

        if (core.Wearer.CurJobDef != RimWorld.JobDefOf.AttackMelee)
        {
            return;
        }

        var pawn = core.Wearer.CurJob.targetA.Pawn;
        if (pawn == null)
        {
            return;
        }

        var skillObject = core.GetComp<CompSkills>().FindSkill("Sweep");
        if (skillObject != null && skillObject.CanUsed(out _))
        {
            skillObject.VerbTracker.PrimaryVerb.TryStartCastOn(pawn.Position);
        }
    }

    public override void GetHurt_Machine(DamageInfo dinfo)
    {
        GetHurt(dinfo);
    }

    public override void GetHurt_Shield(DamageInfo dinfo)
    {
        GetHurt(dinfo);
    }

    private void GetHurt(DamageInfo dinfo)
    {
        if (!dinfo.Def.harmsHealth || !dinfo.IsRangedDamage(core.Wearer))
        {
            return;
        }

        var skillObject = core.GetComp<CompSkills>().FindSkill("VigorouslyRebound");
        if (skillObject != null && skillObject.CanUsed(out _))
        {
            (skillObject.VerbTracker.PrimaryVerb as Verb_VigorouslyRebound)?.StartSkill();
        }
    }
}