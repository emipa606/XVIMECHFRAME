using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace MParmorLibrary;

public class Building_FabricationPit : Building_WorkTableWithActions, IChargingEquipment
{
    private const int FabricationTime = 30000;

    public static List<Building_FabricationPit> Cache = [];

    private int chargingPower = 7;

    private CompPowerTrader compPower;

    public int fabricationTimeLate;

    private MParmorBuilding mParmor;

    public MechanicalArmorDef productionMParmor;

    private RecipeDef recipeDef;

    private Building unfinishedThing;

    public FabricationState State { get; private set; } = FabricationState.Free;

    public Building_FabricationCentralSystem CentralSystem
    {
        get
        {
            foreach (var item in Building_FabricationCentralSystem.Cache)
            {
                if (item.Map == Map && item.Position.InHorDistOf(Position, 15.9f) && item.CompPower.PowerOn)
                {
                    return item;
                }
            }

            return null;
        }
    }

    public int ChargingPower
    {
        get => chargingPower;
        set
        {
            chargingPower = value;
            UpdateState();
        }
    }

    public void UpdateState()
    {
        mParmor = null;
        State = FabricationState.Free;
        if (fabricationTimeLate > 0)
        {
            State = FabricationState.Fabricating;
        }

        if ((from x in Position.GetThingList(Map)
                where x is MParmorBuilding
                select x).FirstOrDefault() is MParmorBuilding mParmorBuilding)
        {
            mParmor = mParmorBuilding;
            State = mParmor.PowerTracker.IsFull ? FabricationState.Full : FabricationState.Charging;
        }

        switch (State)
        {
            case FabricationState.Free:
            case FabricationState.Full:
                compPower.PowerOutput = 0f - compPower.Props.PowerConsumption;
                break;
            case FabricationState.Fabricating:
                compPower.PowerOutput = -700f;
                break;
            default:
                compPower.PowerOutput = ChargingPower * -200f;
                break;
        }
    }

    public override string GetInspectString()
    {
        var stringBuilder = new StringBuilder();
        if (CentralSystem != null)
        {
            stringBuilder.AppendLine("XFMParmor_Building_FabricationPit_ConnectedA".Translate());
        }
        else
        {
            stringBuilder.AppendLine("XFMParmor_Building_FabricationPit_ConnectedB".Translate());
        }

        switch (State)
        {
            case FabricationState.Free:
                stringBuilder.AppendLine("XFMParmor_Building_FabricationPit_StateA".Translate());
                break;
            case FabricationState.Charging:
                stringBuilder.AppendLine("XFMParmor_Building_FabricationPit_StateB".Translate());
                break;
            case FabricationState.Fabricating:
                stringBuilder.AppendLine(
                    "XFMParmor_Building_FabricationPit_StateC".Translate(
                        (fabricationTimeLate / 60000f).ToString("0.#")));
                break;
        }

        stringBuilder.AppendLine("XFMParmor_Building_FabricationPit_Power".Translate(chargingPower * 200));
        stringBuilder.Append(base.GetInspectString());
        return stringBuilder.ToString();
    }

    public static void DrawGrostLine(ThingDef myDef, IntVec3 myPos, Rot4 myRot, Map map)
    {
        var a = GenThing.TrueCenter(myPos, myRot, myDef.size, myDef.Altitude);
        foreach (var item in Building_FabricationCentralSystem.Cache)
        {
            if (item.Map == map && item.Position.InHorDistOf(myPos, 15.9f))
            {
                GenDraw.DrawLineBetween(a, item.TrueCenter());
            }
        }
    }

    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        base.SpawnSetup(map, respawningAfterLoad);
        compPower = GetComp<CompPowerTrader>();
        UpdateState();
        Cache.Add(this);
    }

    public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
    {
        if (unfinishedThing is { Spawned: true })
        {
            unfinishedThing.Destroy();
        }

        Cache.Remove(this);
        base.DeSpawn(mode);
    }

    public override void FinishBill(RecipeDef recipeDef, Pawn worker, List<Thing> ingredients)
    {
        if (!MechanicalArmorDef.recipes.TryGetValue(recipeDef, out var armor))
        {
            return;
        }

        productionMParmor = armor;
        this.recipeDef = recipeDef;
        var building = ThingMaker.MakeThing(ThingDefOf.XFMParmor_UnfinishedMParmor) as Building;
        building?.SetFactionDirect(Faction);
        GenPlace.TryPlaceThing(building, Position, Map, ThingPlaceMode.Direct, null, null, default(Rot4));
        unfinishedThing = building;
        fabricationTimeLate = 30000;
        UpdateState();
    }

    protected override void Tick()
    {
        base.Tick();
        if (!Fabricating())
        {
            Charging();
        }
    }

    private void Charging()
    {
        if (mParmor == null)
        {
            return;
        }

        if (State == FabricationState.Charging && mParmor.PowerTracker.IsFull)
        {
            UpdateState();
        }
        else if (compPower.PowerOn)
        {
            mParmor.PowerTracker.ChargeBatteryExactly(chargingPower * 200 * 100 / 2500);
        }
    }

    private bool Fabricating()
    {
        if (fabricationTimeLate <= 0)
        {
            return false;
        }

        if (!compPower.PowerOn || CentralSystem == null)
        {
            return true;
        }

        fabricationTimeLate--;
        if (fabricationTimeLate == 0)
        {
            FinishFabricating();
        }

        return true;
    }

    private void FinishFabricating()
    {
        if (unfinishedThing is { Spawned: true })
        {
            unfinishedThing.Destroy();
        }

        var mParmorBuilding = ThingMaker.MakeThing(productionMParmor.building) as MParmorBuilding;
        GenPlace.TryPlaceThing(mParmorBuilding, Position, Map, ThingPlaceMode.Direct, null, null, default(Rot4));
        mParmorBuilding?.HealthTracker.Machine = productionMParmor.machine;
        mParmorBuilding?.SetFactionDirect(Faction);
        recipeDef = null;
        productionMParmor = null;
        UpdateState();
    }

    private void StopFabricating()
    {
        if (unfinishedThing is { Spawned: true })
        {
            unfinishedThing.Destroy();
        }

        foreach (var ingredient in recipeDef.ingredients)
        {
            ToolsLibrary.SpawnThingDefCount(
                new ThingDefCountClass(ingredient.FixedIngredient, (int)ingredient.GetBaseCount()), Position, Map);
        }

        recipeDef = null;
        productionMParmor = null;
        fabricationTimeLate = 0;
        UpdateState();
    }

    public override IEnumerable<Gizmo> GetGizmos()
    {
        foreach (var gizmo in base.GetGizmos())
        {
            yield return gizmo;
        }

        yield return new Command_SetChargingPower
        {
            equipment = this,
            defaultLabel = "XFMParmor_Building_ChargingStation_GetGizmosA".Translate(),
            defaultDesc = "XFMParmor_Building_ChargingStation_GetGizmosB".Translate(),
            icon = Texture2DOf.SetTargetFuelLevelCommand
        };
        if (State == FabricationState.Fabricating)
        {
            yield return new Command_Action
            {
                Order = 10f,
                defaultLabel = "XFMParmor_Building_FabricationPit_GizmoC".Translate(),
                icon = Texture2DOf.GetOut,
                action = StopFabricating
            };
        }
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref fabricationTimeLate, "fabricationTimeLate");
        Scribe_Values.Look(ref chargingPower, "chargingPower", 7);
        Scribe_References.Look(ref unfinishedThing, "unfinishedThing");
        Scribe_Defs.Look(ref productionMParmor, "productionMParmor");
        Scribe_Defs.Look(ref recipeDef, "recipeDef");
    }
}