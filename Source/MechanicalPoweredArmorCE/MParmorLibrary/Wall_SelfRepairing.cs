using System.Collections.Generic;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace MParmorLibrary;

public class Wall_SelfRepairing : Building
{
    private const int tick = 80;

    private byte outData;
    private float repairAmout;

    public bool NotAllowFix { get; private set; } = true;

    public Color HitPointColor => new(HitPoints / (float)MaxHitPoints, 0f, 0f);

    public bool CanSelfRepairing
    {
        get
        {
            if (PowerComp == null || ((CompPowerTrader)PowerComp).PowerOn)
            {
                return MaxHitPoints > HitPoints;
            }

            return false;
        }
    }

    public int HealAmoutPerSecond
    {
        get
        {
            var num = repairAmout;
            num /= 1.3333334f;
            num += outData / 100f;
            var num2 = (int)num;
            outData = (byte)((num - num2) * 100f);
            return num2;
        }
    }

    protected override void Tick()
    {
        base.Tick();
        if (Find.TickManager.TicksGame % 80 == 0)
        {
            TickNew();
        }
    }

    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        base.SpawnSetup(map, respawningAfterLoad);
        var num = MaxHitPoints > def.GetStatValueAbstract(StatDefOf.MaxHitPoints)
            ? MaxHitPoints / 300f
            : def.GetStatValueAbstract(StatDefOf.MaxHitPoints) / 300f;
        if (num > 9f)
        {
            num = 9f;
        }

        repairAmout = num;
    }

    public override string GetInspectString()
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append(base.GetInspectString());
        if (!CanSelfRepairing)
        {
            return stringBuilder.ToString();
        }

        if (stringBuilder.Length > 0)
        {
            stringBuilder.AppendLine();
        }

        stringBuilder.Append("XFMParmor_Wall_SelfRepairing".Translate(
            ((MaxHitPoints - (float)HitPoints) / (repairAmout * 125f / 3f)).ToString("0.#")));

        return stringBuilder.ToString();
    }

    public virtual void TickNew()
    {
        if (!CanSelfRepairing)
        {
            return;
        }

        if (HitPoints + HealAmoutPerSecond > MaxHitPoints)
        {
            HitPoints = MaxHitPoints;
        }
        else
        {
            HitPoints += HealAmoutPerSecond;
        }
    }

    public override IEnumerable<Gizmo> GetGizmos()
    {
        foreach (var gizmo in base.GetGizmos())
        {
            yield return gizmo;
        }

        if (Faction == Faction.OfPlayer)
        {
            yield return new Command_Toggle
            {
                isActive = () => !NotAllowFix,
                defaultLabel = "XFMParmor_Wall_SelfRepairing_Gizmo".Translate(),
                defaultDesc = "XFMParmor_Wall_SelfRepairing_Gizmod".Translate(),
                activateIfAmbiguous = true,
                toggleAction = delegate { NotAllowFix = !NotAllowFix; }
            };
        }
    }
}