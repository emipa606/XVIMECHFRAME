using System.Collections.Generic;
using RimWorld;
using Verse;

namespace MParmorLibrary;

public class Building_FabricationCentralSystem : Building
{
    public static List<Building_FabricationCentralSystem> Cache = [];

    public CompPowerTrader CompPower => field ?? (field = GetComp<CompPowerTrader>());

    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        base.SpawnSetup(map, respawningAfterLoad);
        Cache.Add(this);
    }

    public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
    {
        base.DeSpawn(mode);
        Cache.Remove(this);
    }

    public static void DrawGrostLine(ThingDef myDef, IntVec3 myPos, Rot4 myRot, Map map)
    {
        var a = GenThing.TrueCenter(myPos, myRot, myDef.size, myDef.Altitude);
        foreach (var item in Building_FabricationPit.Cache)
        {
            if (item.Map == map && item.Position.InHorDistOf(myPos, 15.9f))
            {
                GenDraw.DrawLineBetween(a, item.TrueCenter());
            }
        }
    }
}