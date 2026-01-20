using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace MParmorLibrary.Patches;

[HarmonyPatch(typeof(GenRecipe), nameof(GenRecipe.MakeRecipeProducts), typeof(RecipeDef), typeof(Pawn),
    typeof(List<Thing>),
    typeof(Thing), typeof(IBillGiver), typeof(Precept_ThingStyle), typeof(ThingStyleDef), typeof(int?))]
public static class GenRecipe_MakeRecipeProducts
{
    private static void Prefix(RecipeDef recipeDef, IBillGiver billGiver, Pawn worker, List<Thing> ingredients)
    {
        if (billGiver is Building_WorkTableWithActions building_WorkTableWithActions)
        {
            building_WorkTableWithActions.FinishBill(recipeDef, worker, ingredients);
        }

        if (recipeDef.Worker is RecipeWorker_Maintain recipeWorker_Maintain &&
            billGiver is MParmorBuilding building)
        {
            recipeWorker_Maintain.Maintain(building, ingredients);
        }
    }
}