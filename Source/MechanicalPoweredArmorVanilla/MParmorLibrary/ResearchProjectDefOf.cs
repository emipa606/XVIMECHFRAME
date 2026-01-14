using RimWorld;
using Verse;

namespace MParmorLibrary;

[DefOf]
public static class ResearchProjectDefOf
{
    public static ResearchProjectDef XFMParmor_Root;

    static ResearchProjectDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(ResearchProjectDefOf));
    }
}