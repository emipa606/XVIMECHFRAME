using System.Collections.Generic;
using System.Linq;
using MParmorLibrary.Settings;
using MParmorLibrary.SingleObject;
using RimWorld;
using Verse;

namespace MParmorLibrary;

public static class ToolsLibrary_MParmorOnly
{
    public static bool IsUnfriendly(DamageInfo dinfo, Thing victim)
    {
        return IsUnfriendly(dinfo.Instigator, victim);
    }

    public static bool IsUnfriendly(Thing a, Thing b)
    {
        if (a == b)
        {
            return false;
        }

        if (Main.Instance.settings.forceFriendlyFire)
        {
            return true;
        }

        return a == null || b == null || a.Faction == null || b.Faction == null || a.Faction.HostileTo(b.Faction);
    }

    public static bool IsUnfriendly(Projectile a, Thing b)
    {
        if (a.Launcher == null)
        {
            return true;
        }

        if (a.Launcher == b)
        {
            return false;
        }

        if (Main.Instance.settings.forceFriendlyFire)
        {
            return true;
        }

        return b == null || a.Launcher.Faction == null || b.Faction == null || a.Launcher.Faction.HostileTo(b.Faction);
    }

    public static bool IsUnfriendly(Faction a, Faction b)
    {
        if (a == b)
        {
            return false;
        }

        if (Main.Instance.settings.forceFriendlyFire)
        {
            return true;
        }

        return a == null || b == null || a.HostileTo(b);
    }

    public static List<MParmorCore> GetMParmor(Map map = null)
    {
        var list = new List<MParmorCore>();
        list.AddRange(AcquisitionManagement.GetInstance().cores.Where(core =>
            map == null || core.Wearer.Map == map || (core.Wearer.ParentHolder as Thing)?.Map == map));
        return list;
    }

    public static List<Thing> GetMParmorThing(Map map = null)
    {
        var list = new List<Thing>();
        foreach (var item in AcquisitionManagement.GetInstance().cores.Where(core =>
                     map == null || core.Wearer.Map == map || (core.Wearer.ParentHolder as Thing)?.Map == map))
        {
            list.Add(item);
        }

        return list;
    }

    public static List<MParmorBuilding> GetMParmorBuilding(Map map = null)
    {
        var list = new List<MParmorBuilding>();
        list.AddRange(MParmorBuilding.Cache.Where(core => map == null || core.Map == map));
        return list;
    }

    extension(Pawn pawn)
    {
        public bool GetMParmorCore(out MParmorCore core)
        {
            IEnumerable<Apparel> enumerable = pawn.apparel?.WornApparel;
            foreach (var item in enumerable ?? [])
            {
                if (item is not MParmorCore mParmorCore)
                {
                    continue;
                }

                core = mParmorCore;
                return true;
            }

            core = null;
            return false;
        }

        public bool GetMParmorSelfDestruct(out MParmorSelfDestruct core)
        {
            IEnumerable<Apparel> enumerable = pawn.apparel?.WornApparel;
            foreach (var item in enumerable ?? [])
            {
                if (item is not MParmorSelfDestruct mParmorSelfDestruct)
                {
                    continue;
                }

                core = mParmorSelfDestruct;
                return true;
            }

            core = null;
            return false;
        }

        public bool GetMParmorCore()
        {
            return pawn.GetMParmorCore(out _);
        }

        public bool GetMParmorSelfDestruct()
        {
            return pawn.GetMParmorSelfDestruct(out _);
        }
    }
}