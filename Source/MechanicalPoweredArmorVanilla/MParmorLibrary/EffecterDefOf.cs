using RimWorld;
using Verse;

namespace MParmorLibrary;

[DefOf]
public static class EffecterDefOf
{
    public static EffecterDef XFMParmor_EffectExplosion;

    public static EffecterDef XFMParmor_ShieldChipeEffect;

    static EffecterDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(EffecterDefOf));
    }
}