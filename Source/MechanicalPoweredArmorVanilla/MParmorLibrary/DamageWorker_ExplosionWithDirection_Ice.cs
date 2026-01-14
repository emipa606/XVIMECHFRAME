using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace MParmorLibrary;

public class DamageWorker_ExplosionWithDirection_Ice : DamageWorker_AddInjury
{
    public static float? direction;

    public static float angle = 360f;

    public override void ExplosionStart(Explosion explosion, List<IntVec3> cellsToAffect)
    {
    }

    public override void ExplosionAffectCell(Explosion explosion, IntVec3 c, List<Thing> damagedThings,
        List<Thing> ignoredThings, bool canThrowMotes)
    {
        if (c.GetFirstBuilding(explosion.Map) == null &&
            ((c - explosion.Position).LengthHorizontalSquared < 6.25f || Rand.Chance(0.2f)))
        {
            var building_IceSpike = ThingMaker.MakeThing(ThingDefOf.XFMParmor_IceSpike) as Building_IceSpike;
            building_IceSpike?.owner = explosion.instigator;
            building_IceSpike?.SetFaction(explosion.instigator.Faction);
            GenPlace.TryPlaceThing(building_IceSpike, c, explosion.Map, ThingPlaceMode.Direct, null, null,
                default(Rot4));
        }

        if (!c.ShouldSpawnMotesAt(explosion.Map))
        {
            return;
        }

        float rotation = 90 * Rand.RangeInclusive(0, 3);
        var dataStatic = FleckMaker.GetDataStatic(c.ToVector3Shifted(), explosion.Map,
            FleckDefOf.XFMParmor_Fleck_BlastDryA);
        var dataStatic2 = FleckMaker.GetDataStatic(c.ToVector3Shifted(), explosion.Map,
            FleckDefOf.XFMParmor_Fleck_BlastDryB);
        dataStatic.exactScale = new Vector3(1f, 5f, 1f);
        dataStatic.rotation = rotation;
        dataStatic.instanceColor = new Color32(79, 249, 239, byte.MaxValue);
        dataStatic2.rotation = rotation;
        dataStatic2.instanceColor = Color.white;
        explosion.Map.flecks.CreateFleck(dataStatic);
        explosion.Map.flecks.CreateFleck(dataStatic2);
    }

    public override IEnumerable<IntVec3> ExplosionCellsToHit(IntVec3 center, Map map, float radius,
        IntVec3? needLOSToCell1 = null, IntVec3? needLOSToCell2 = null, FloatRange? affectedAngle = null)
    {
        var list = new List<IntVec3>();
        foreach (var item in base.ExplosionCellsToHit(center, map, radius, needLOSToCell1, needLOSToCell2,
                     affectedAngle))
        {
            if (item == center)
            {
                continue;
            }

            if ((item - center).LengthHorizontalSquared < 6.25f)
            {
                list.Add(item);
            }
            else if (direction != null && ToolsLibrary.IsInSector(center.ToVector3Shifted(), item.ToVector3Shifted(),
                         direction.Value,
                         angle))
            {
                list.Add(item);
            }
        }

        direction = null;
        angle = 360f;
        return list;
    }
}