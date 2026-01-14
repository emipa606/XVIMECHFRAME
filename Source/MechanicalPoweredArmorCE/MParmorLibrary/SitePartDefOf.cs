using RimWorld;

namespace MParmorLibrary;

[DefOf]
public static class SitePartDefOf
{
    public static SitePartDef XFMPArmor_SleepingMechanoids;

    static SitePartDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(SitePartDefOf));
    }
}