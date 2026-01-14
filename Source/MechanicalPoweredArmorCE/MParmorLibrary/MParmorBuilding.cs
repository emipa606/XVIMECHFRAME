using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace MParmorLibrary;

public class MParmorBuilding : Building_WorkTable, IMParmorInstance
{
    public static List<MParmorBuilding> Cache = [];

    public CompMParmorBuilding comp;

    private MParmorT_HealthTracker healthTracker;

    private MParmorT_ModulesTracker modulesTracker;

    private MParmorT_PowerTracker powerTracker;

    public MechanicalArmorDef System => (comp ?? (comp = GetComp<CompMParmorBuilding>())).Props.mechanicalArmor;

    public MParmorT_PowerTracker PowerTracker => powerTracker ?? (powerTracker = new MParmorT_PowerTracker(this));

    public MParmorT_HealthTracker HealthTracker => healthTracker ?? (healthTracker = new MParmorT_HealthTracker(this));

    public MParmorT_ModulesTracker ModulesTracker =>
        modulesTracker ?? (modulesTracker = new MParmorT_ModulesTracker(this));

    public void CopyTracker(IMParmorInstance instance)
    {
        powerTracker = instance.PowerTracker;
        powerTracker.instance = this;
        healthTracker = instance.HealthTracker;
        healthTracker.instance = this;
        modulesTracker = instance.ModulesTracker;
        modulesTracker.instance = this;
    }

    public override IEnumerable<Gizmo> GetGizmos()
    {
        yield return new Gizmo_MParmorBuilding
        {
            core = this,
            healthTracker = HealthTracker,
            powerTracker = PowerTracker
        };
        foreach (var gizmo in base.GetGizmos())
        {
            yield return gizmo;
        }
    }

    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        comp = GetComp<CompMParmorBuilding>();
        base.SpawnSetup(map, respawningAfterLoad);
        if ((from x in Position.GetThingList(Map)
                where x is Building_FabricationPit
                select x).FirstOrDefault() is Building_FabricationPit building_FabricationPit)
        {
            building_FabricationPit.UpdateState();
        }

        Cache.Add(this);
    }

    public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
    {
        var position = Position;
        var map = Map;
        base.DeSpawn(mode);
        if ((from x in position.GetThingList(map)
                where x is Building_FabricationPit
                select x).FirstOrDefault() is Building_FabricationPit building_FabricationPit)
        {
            building_FabricationPit.UpdateState();
        }

        Cache.Remove(this);
    }

    protected override void Tick()
    {
        base.Tick();
        ModulesTracker.TickBuilding();
    }

    protected override void DrawAt(Vector3 drawLoc, bool flip = false)
    {
        base.DrawAt(drawLoc, flip);
        ModulesTracker.DrawBuilding(drawLoc);
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Deep.Look(ref powerTracker, "powerTracker", this);
        Scribe_Deep.Look(ref healthTracker, "healthTracker", this);
        Scribe_Deep.Look(ref modulesTracker, "modulesTracker", this);
    }
}