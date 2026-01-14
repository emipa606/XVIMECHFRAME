using RimWorld;
using Verse;

namespace MParmorLibrary;

[DefOf]
public static class RecipeDefOf
{
    public static RecipeDef XFMParmor_FixMachine;

    public static RecipeDef XFMParmor_ChangePowerCell;

    static RecipeDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(RecipeDefOf));
    }
}