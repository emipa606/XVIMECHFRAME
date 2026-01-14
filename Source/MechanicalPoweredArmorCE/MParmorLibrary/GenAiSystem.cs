using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace MParmorLibrary;

public static class GenAiSystem
{
    private static readonly List<MParmorCore> cache = [];

    public static List<Pawn> GetTargets(Pawn host, Map map, float? distance = null, bool canDown = false,
        bool mustCanReach = true)
    {
        return map.mapPawns.AllPawnsSpawned.Where(x => CanBeATarget(x, host, distance, canDown, mustCanReach)).ToList();
    }

    private static bool CanBeATarget(Pawn target, Pawn host, float? distance = null, bool canDown = false,
        bool mustCanReach = true)
    {
        return host.HostileTo(target) && (canDown || !target.Downed) &&
               (!distance.HasValue || ToolsLibrary.GetDistance(target.DrawPos, host.DrawPos) < distance * distance) &&
               (!mustCanReach || host.Map.reachability.CanReach(host.Position, target, PathEndMode.ClosestTouch,
                   TraverseParms.For(target)));
    }

    public static bool TryFindNearestMParmor_BeHit(this Pawn pawn, int timeDelay, out MParmorCore core)
    {
        cache.Clear();
        core = null;
        foreach (var item in ToolsLibrary_MParmorOnly.GetMParmor(pawn.Map))
        {
            if (Find.TickManager.TicksGame - item.Wearer.mindState.lastHarmTick <= timeDelay)
            {
                cache.Add(item);
            }
        }

        if (!cache.Any())
        {
            return false;
        }

        cache.SortBy(coreE => ToolsLibrary.GetDistance(coreE.Wearer.Position, pawn.Position));
        core = cache.First();
        return true;
    }

    extension(DamageInfo dinfo)
    {
        public bool IsRangedDamage(Thing victim)
        {
            if (dinfo.Instigator is not { Spawned: true } || dinfo.Instigator == victim)
            {
                return false;
            }

            return ToolsLibrary.GetDistance(dinfo.Instigator.Position, victim.Position) > 2.1;
        }

        public bool IsMeleeDamage(Thing victim)
        {
            if (dinfo.Instigator is not { Spawned: true } || dinfo.Instigator == victim)
            {
                return false;
            }

            return ToolsLibrary.GetDistance(dinfo.Instigator.Position, victim.Position) < 2.1;
        }
    }
}