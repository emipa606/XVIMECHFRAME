using RimWorld;

namespace MParmorLibrary;

[DefOf]
public static class RaidStrategyDefOf
{
    public static RaidStrategyDef XFMParmor_RaidStrategy_AntiMPArmor;

    static RaidStrategyDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(RaidStrategyDefOf));
    }
}