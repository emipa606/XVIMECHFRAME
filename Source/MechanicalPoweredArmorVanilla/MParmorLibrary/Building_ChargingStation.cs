using System;
using System.Collections.Generic;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace MParmorLibrary;

public class Building_ChargingStation : Building, IChargingEquipment
{
    public const int batteryMax = 20000000;
    private int batteryLevel = -1;

    private int chargingPower = 7;

    private CompPowerTrader compPower;

    public override Graphic Graphic
    {
        get
        {
            if (!HasBattery)
            {
                return base.Graphic;
            }

            if (field != null)
            {
                return field;
            }

            var graphicData = new GraphicData();
            graphicData.CopyFrom(def.graphicData);
            graphicData.texPath = "XFMParmor/Buildings/ChargeStation_Full";
            field = graphicData.Graphic;
            return field;
        }
    }

    public bool HasBattery => batteryLevel != -1;

    public bool IsBatteryFull => batteryLevel >= 20000000;

    public int BatteryLevelOut => Math.Min(batteryLevel, 20000000);

    public int ChargingPower
    {
        get => chargingPower;
        set => chargingPower = value;
    }

    public override IEnumerable<Gizmo> GetGizmos()
    {
        yield return new Command_SetChargingPower
        {
            equipment = this,
            defaultLabel = "XFMParmor_Building_ChargingStation_GetGizmosA".Translate(),
            defaultDesc = "XFMParmor_Building_ChargingStation_GetGizmosB".Translate(),
            icon = Texture2DOf.SetTargetFuelLevelCommand
        };
        if (Prefs.DevMode)
        {
            yield return new Command_Action
            {
                defaultLabel = "Charge 10000Wh",
                action = delegate { batteryLevel += 1000000; }
            };
        }

        foreach (var gizmo in base.GetGizmos())
        {
            yield return gizmo;
        }
    }

    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        base.SpawnSetup(map, respawningAfterLoad);
        compPower = GetComp<CompPowerTrader>();
    }

    public override string GetInspectString()
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine(base.GetInspectString());
        stringBuilder.AppendLine("XFMParmor_Building_ChargingStation_GetInspectStringA".Translate(chargingPower * 200));
        stringBuilder.Append(
            "XFMParmor_Building_ChargingStation_GetInspectStringB".Translate((int)(batteryLevel / 100f)));
        return stringBuilder.ToString();
    }

    public void FillBattery(PowerCell powerCell)
    {
        batteryLevel = powerCell.batteryLevel;
        Notify_ColorChanged();
        powerCell.Destroy();
    }

    public Thing TakeBattery()
    {
        if (IsBatteryFull)
        {
            Reset();
            return ThingMaker.MakeThing(ThingDefOf.XFMParmor_FilledPowerCell);
        }

        var powerCell = (PowerCell)ThingMaker.MakeThing(ThingDefOf.XFMParmor_FilledPowerCell);
        powerCell.batteryLevel = batteryLevel;
        Reset();
        return powerCell;
    }

    private void Reset()
    {
        batteryLevel = -1;
        Notify_ColorChanged();
    }

    protected override void Tick()
    {
        base.Tick();
        WorkTick();
    }

    private void WorkTick()
    {
        if (HasBattery && !IsBatteryFull)
        {
            if (compPower.PowerOn)
            {
                batteryLevel += chargingPower * 20000 / 2500;
            }

            compPower.PowerOutput = chargingPower * -200f;
            if (IsBatteryFull)
            {
                batteryLevel = 20000000;
            }
        }
        else
        {
            compPower.PowerOutput = 0f - compPower.Props.PowerConsumption;
        }
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref batteryLevel, "pobatteryLevel", -1);
        Scribe_Values.Look(ref chargingPower, "chargingPower", 7);
    }

    protected override void DrawAt(Vector3 drawLoc, bool flip = false)
    {
        base.DrawAt(drawLoc, flip);
        if (HasBattery)
        {
            GenDraw.DrawFillableBar(new GenDraw.FillableBarRequest
            {
                center = DrawPos + new Vector3(-0.33f, 0f, 0.15f),
                size = new Vector2(0.39f, 0.06f),
                fillPercent = batteryLevel / 20000000f,
                filledMat = Colors.powerBarTex_Material,
                rotation = Rot4.East
            });
        }
    }
}