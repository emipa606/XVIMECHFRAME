using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace MParmorLibrary.Unused;

public class DamageWorker_ExplosionWithDirection : DamageWorker_AddInjury
{
    public static float? direction;

    public static float angle = 360f;

    public override void ExplosionStart(Explosion explosion, List<IntVec3> cellsToAffect)
    {
    }

    public override void ExplosionAffectCell(Explosion explosion, IntVec3 c, List<Thing> damagedThings,
        List<Thing> ignoredThings, bool canThrowMotes)
    {
        base.ExplosionAffectCell(explosion, c, damagedThings, ignoredThings, false);
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
        if (!direction.HasValue || angle == 360f)
        {
            return base.ExplosionCellsToHit(center, map, radius, needLOSToCell1, needLOSToCell2, affectedAngle);
        }

        var list = new List<IntVec3>();
        foreach (var item in base.ExplosionCellsToHit(center, map, radius, needLOSToCell1, needLOSToCell2,
                     affectedAngle))
        {
            if (ToolsLibrary.IsInSector(center.ToVector3Shifted(), item.ToVector3Shifted(), direction.Value, angle))
            {
                list.Add(item);
            }
        }

        direction = null;
        angle = 360f;
        return list;
    }
}