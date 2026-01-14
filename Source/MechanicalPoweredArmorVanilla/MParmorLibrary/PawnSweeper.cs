using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace MParmorLibrary;

public class PawnSweeper : Thing, IThingHolder, IAttackTarget
{
    private ThingOwner<Thing> innerContainer;

    private JobQueue jobQueue;

    private Vector3 lastPos;

    private bool pawnWasDrafted;

    private bool pawnWasSelected;

    private int spawnTick;

    private List<Pawn> targets;

    public PawnSweeper()
    {
        innerContainer = new ThingOwner<Thing>(this);
    }

    public Pawn Pawn
    {
        get
        {
            if (innerContainer.InnerListForReading.Count <= 0)
            {
                return null;
            }

            return innerContainer.InnerListForReading[0] as Pawn;
        }
    }

    Thing IAttackTarget.Thing => this;

    LocalTargetInfo IAttackTarget.TargetCurrentlyAimingAt => LocalTargetInfo.Invalid;

    float IAttackTarget.TargetPriorityFactor => 1f;

    bool IAttackTarget.ThreatDisabled(IAttackTargetSearcher disabledFor)
    {
        return !Spawned;
    }

    public ThingOwner GetDirectlyHeldThings()
    {
        return innerContainer;
    }

    public void GetChildHolders(List<IThingHolder> outChildren)
    {
        ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, GetDirectlyHeldThings());
    }

    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        base.SpawnSetup(map, respawningAfterLoad);
        spawnTick = Find.TickManager.TicksGame;
    }

    protected override void Tick()
    {
        if ((Find.TickManager.TicksGame - spawnTick) % 6 == 0)
        {
            Affect();
        }
    }

    private void Affect()
    {
        if (targets.Count > 0)
        {
            var index = targets.Count - 1;
            var pawn = targets[index];
            targets.RemoveAt(index);
            if (pawn is not { Spawned: true })
            {
                Affect();
                return;
            }

            ShowFleck(pawn.DrawPos);
            ShowHitFleck(pawn.DrawPos);
            lastPos = pawn.DrawPos;
            if (targets.Count == 0 && lastPos.ToIntVec3() == Position)
            {
                Destroy();
            }

            pawn.TakeDamage(new DamageInfo(RimWorld.DamageDefOf.Cut, 19f, 0.6f,
                (pawn.DrawPos - Position.ToVector3Shifted()).AngleFlat(), Pawn));
        }
        else
        {
            ShowFleck(Position.ToVector3Shifted());
            Destroy();
        }
    }

    private void End()
    {
        var pawn = Pawn;
        innerContainer.TryDrop(pawn, Position, pawn.MapHeld, ThingPlaceMode.Direct, out _, null, null, false);
        pawn.drafter?.Drafted = pawnWasDrafted;

        if (pawnWasSelected && Find.CurrentMap == pawn.Map)
        {
            Find.Selector.Select(pawn, false);
        }

        if (jobQueue != null)
        {
            pawn.jobs.RestoreCapturedJobs(jobQueue);
        }
    }

    public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
    {
        CheckDestination();
        End();
        base.Destroy(mode);
    }

    private void ShowHitFleck(Vector3 pos)
    {
        Map.flecks.CreateFleck(new FleckCreationData
        {
            def = FleckDefOf.XFMParmor_SweepHitFleck,
            scale = 3f,
            spawnPosition = pos,
            rotation = Rand.Range(0f, 360f)
        });
        Map.flecks.CreateFleck(new FleckCreationData
        {
            def = FleckDefOf.XFMParmor_SweepHitFleck,
            scale = 1.5f,
            spawnPosition = pos,
            rotation = Rand.Range(0f, 360f)
        });
    }

    private void ShowFleck(Vector3 pos)
    {
        var vector = pos - lastPos;
        var num = vector.MagnitudeHorizontal();
        var dataStatic = FleckMaker.GetDataStatic(lastPos + (vector * 0.5f), Map, FleckDefOf.XFMParmor_SweepFleck);
        dataStatic.exactScale = new Vector3(num + 1f, 1f, 2f);
        dataStatic.rotation = Mathf.Atan2(0f - vector.z, vector.x) * 57.29578f;
        Map.flecks.CreateFleck(dataStatic);
    }

    private void CheckDestination()
    {
        if (JumpUtility.ValidJumpTarget(this, Map, Position))
        {
            return;
        }

        var num = GenRadial.NumCellsInRadius(3.9f);
        for (var i = 0; i < num; i++)
        {
            var intVec = Position + GenRadial.RadialPattern[i];
            if (!JumpUtility.ValidJumpTarget(this, Map, intVec))
            {
                continue;
            }

            Position = intVec;
            break;
        }
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Deep.Look(ref innerContainer, "innerContainer", this);
        Scribe_Values.Look(ref lastPos, "lastPos");
        Scribe_Values.Look(ref pawnWasDrafted, "pawnWasDrafted");
        Scribe_Values.Look(ref pawnWasSelected, "pawnWasSelected");
        Scribe_Values.Look(ref spawnTick, "spawnTick");
        Scribe_Collections.Look(ref targets, "targets", LookMode.Reference);
        Scribe_Deep.Look(ref jobQueue, "jobQueue");
    }

    public static PawnSweeper MakeSweeper(Pawn pawn, Vector3 startPos, List<Pawn> targets)
    {
        var pawnSweeper = (PawnSweeper)ThingMaker.MakeThing(ThingDefOf.XFMParmor_PawnSweeper);
        pawnSweeper.targets = targets;
        pawnSweeper.lastPos = startPos;
        pawnSweeper.pawnWasDrafted = pawn.Drafted;
        pawnSweeper.pawnWasSelected = Find.Selector.IsSelected(pawn);
        pawnSweeper.jobQueue = pawn.jobs.CaptureAndClearJobQueue();
        pawn.DeSpawn();
        if (pawnSweeper.innerContainer.TryAdd(pawn))
        {
            return pawnSweeper;
        }

        Log.Error($"Could not add {pawn.ToStringSafe()} to a flyer.");
        pawn.Destroy();

        return pawnSweeper;
    }
}