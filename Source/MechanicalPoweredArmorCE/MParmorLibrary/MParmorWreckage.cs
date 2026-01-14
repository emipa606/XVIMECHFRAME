using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace MParmorLibrary;

public class MParmorWreckage : Building
{
    public static readonly int wickTick = 90;

    public static readonly float radius = 24.9f;

    public Pawn driver;

    public string driverName = "predecessors";

    public int stability;

    public int stabilityMax = 600;
    public MechanicalArmorDef system;

    protected Sustainer wickSoundSustainer;

    public bool IsMisc => def == ThingDefOf.XFMParmor_Wreckage_Misc || def == ThingDefOf.XFMParmor_Wreckage_OutQuest;

    public override void DrawExtraSelectionOverlays()
    {
        base.DrawExtraSelectionOverlays();
        if (stability > 0)
        {
            GenDraw.DrawRadiusRing(Position, radius, new Color(0.30980393f, 83f / 85f, 0.9372549f, 0.75f),
                c => GenSight.LineOfSight(Position, c, Map));
        }
    }

    protected override void Tick()
    {
        base.Tick();
        if (IsMisc || stability == 0)
        {
            return;
        }

        if (stability > 0)
        {
            stability--;
        }

        wickSoundSustainer?.Maintain();

        if (stability == wickTick)
        {
            StartWickSustainer();
        }

        if (stability != 0)
        {
            return;
        }

        EndWickSustainer();
        SelfDestruct();
    }

    public void SelfDestruct()
    {
        GenExplosion.DoExplosion(Position, Map, radius, DamageDefOf.MParmor_SlefDestructBomb, driver, 200, 70f,
            RimWorld.DamageDefOf.Bomb.soundExplosion, null, null, null, null, 0f, 1, null, null, 0);
        var effecter = EffecterDefOf.XFMParmor_EffectExplosion.Spawn();
        var targetInfo = new TargetInfo(Position, Map);
        effecter.Trigger(targetInfo, targetInfo);
        effecter.Cleanup();
    }

    public override IEnumerable<Gizmo> GetGizmos()
    {
        foreach (var gizmo in base.GetGizmos())
        {
            yield return gizmo;
        }

        yield return new Gizmo_MParmorWreckage
        {
            stability = stability,
            stabilityMax = stabilityMax,
            driverName = driver is { Name: not null } ? driver.LabelCap : driverName
        };
    }

    private void StartWickSustainer()
    {
        RimWorld.SoundDefOf.MetalHitImportant.PlayOneShot(new TargetInfo(Position, Map));
        var info = SoundInfo.InMap(this, MaintenanceType.PerTick);
        wickSoundSustainer = RimWorld.SoundDefOf.HissSmall.TrySpawnSustainer(info);
    }

    public void FillWickSustainer()
    {
        var info = SoundInfo.InMap(this, MaintenanceType.PerTick);
        wickSoundSustainer = RimWorld.SoundDefOf.HissSmall.TrySpawnSustainer(info);
    }

    private void EndWickSustainer()
    {
        if (wickSoundSustainer == null)
        {
            return;
        }

        wickSoundSustainer.End();
        wickSoundSustainer = null;
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref stability, "stability");
        Scribe_Values.Look(ref stabilityMax, "stabilityMax", 600);
        Scribe_Values.Look(ref driverName, "driverName", "predecessors");
        Scribe_Defs.Look(ref system, "system");
        Scribe_References.Look(ref driver, "system");
    }
}