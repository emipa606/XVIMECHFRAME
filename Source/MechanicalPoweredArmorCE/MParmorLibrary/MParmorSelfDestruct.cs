using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.Sound;

namespace MParmorLibrary;

public class MParmorSelfDestruct : Apparel, IAntiSuppressable
{
    public static readonly int wickTick = 90;

    public int energy;

    public int stability = 600;

    public int stabilityMax = 600;
    public MechanicalArmorDef system;

    protected Sustainer wickSoundSustainer;

    public bool ForceDraft => true;

    public bool DeclineDraftOrder => true;

    public bool DeclineFireOrder => true;

    public bool CanAntiSuppressable => true;

    public void Restart()
    {
        stability = 600;
        stabilityMax = 600;
    }

    public override void DrawWornExtras()
    {
        var wearer = Wearer;
        if (!wearer.Spawned)
        {
            return;
        }

        var drawPos = wearer.DrawPos;
        drawPos.y += 0.01f;
        system.wreckageGraphic.Graphic.Draw(drawPos, Rot4.North, Wearer);
    }

    protected override void Tick()
    {
        base.Tick();
        if (stability == 0 || Wearer?.Spawned == false)
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

        if (stability == 0)
        {
            SelfDestruct();
        }
    }

    public void SelfDestruct()
    {
        EndWickSustainer();
        if (Wearer == null)
        {
            return;
        }

        if (ThingMaker.MakeThing(system.wreckage) is MParmorWreckage mParmorWreckage)
        {
            mParmorWreckage.system = system;
            mParmorWreckage.stabilityMax = 600;
            mParmorWreckage.stability = Math.Max(stability, 1);
            mParmorWreckage.driver = Wearer;
            mParmorWreckage.driverName = Wearer.Name.ToString();
            mParmorWreckage.SetFactionDirect(Wearer.Faction);
            if (wickSoundSustainer != null)
            {
                mParmorWreckage.FillWickSustainer();
            }

            GenSpawn.Spawn(mParmorWreckage, Wearer.Position, Wearer.Map, WipeMode.VanishOrMoveAside);
        }

        Wearer.equipment.Remove(Wearer.equipment.Primary);
        var wearer = Wearer;
        Wearer.apparel.Remove(this);
        DriverShield.AddShield(wearer, energy);
    }

    public override bool CheckPreAbsorbDamage(DamageInfo dinfo)
    {
        RimWorld.SoundDefOf.EnergyShield_AbsorbDamage.PlayOneShot(new TargetInfo(Wearer.Position, Wearer.Map));
        var loc = Wearer.TrueCenter() + (Vector3Utility.HorizontalVectorFromAngle(dinfo.Angle).RotatedBy(180f) * 0.5f);
        var num = Math.Max(Rand.Range(dinfo.Amount / 70f, dinfo.Amount / 10f), 1f);
        FleckMaker.Static(loc, Wearer.Map, RimWorld.FleckDefOf.ExplosionFlash, num);
        var num2 = (int)num;
        for (var i = 0; i < num2; i++)
        {
            FleckMaker.ThrowDustPuff(loc, Wearer.Map, Rand.Range(0.8f, 1.2f));
        }

        return true;
    }

    public override IEnumerable<Gizmo> GetWornGizmos()
    {
        foreach (var wornGizmo in base.GetWornGizmos())
        {
            yield return wornGizmo;
        }

        if (Find.Selector.SingleSelectedThing == Wearer)
        {
            yield return new Gizmo_MParmorWreckage
            {
                stability = stability,
                stabilityMax = stabilityMax,
                driverName = Wearer.LabelCap
            };
        }

        if (Wearer.Map != null)
        {
            yield return new Command_Action
            {
                Order = 1000f,
                defaultLabel = "退出机甲",
                action = SelfDestruct
            };
        }
    }

    private void StartWickSustainer()
    {
        var targetInfo = (TargetInfo)Wearer;
        if (Wearer == null)
        {
            targetInfo = this;
        }

        RimWorld.SoundDefOf.MetalHitImportant.PlayOneShot(targetInfo);
        var info = SoundInfo.InMap(targetInfo, MaintenanceType.PerTick);
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
        Scribe_Values.Look(ref stability, "stability", -1);
        Scribe_Values.Look(ref stabilityMax, "stabilityMax", -1);
        Scribe_Values.Look(ref energy, "energy");
        Scribe_Deep.Look(ref system, "system", false);
    }
}