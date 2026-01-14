using System.Collections.Generic;
using RimWorld;
using Verse;

namespace MParmorLibrary;

public static class RecipeGenerator
{
    public static void GenerateImpliedDefs_PreResolve()
    {
        foreach (var item in DefsFromRecipeMakers())
        {
            RimWorld.DefGenerator.AddImpliedDef(item);
        }
    }

    private static IEnumerable<RecipeDef> DefsFromRecipeMakers()
    {
        foreach (var def in DefDatabase<MechanicalArmorDef>.AllDefs)
        {
            yield return CreateRecipeDefFromMechanicalArmorDef(def);
        }

        foreach (var def2 in DefDatabase<ModuleDef>.AllDefs)
        {
            yield return CreateRecipeDefFromModuleDef(def2);
        }
    }

    private static RecipeDef CreateRecipeDefFromMechanicalArmorDef(MechanicalArmorDef def)
    {
        var recipeDef = new RecipeDef
        {
            defName = $"XFMParmor_Fabricate_{def.defName}",
            label = "XFMParmor_RecipeDef_label".Translate() + def.label,
            description = def.description,
            jobString = "XFMParmor_RecipeDef_jobString".Translate(),
            workAmount = 100f,
            workSpeedStat = StatDefOf.WorkSpeedGlobal
        };
        foreach (var cost in def.costList)
        {
            var ingredientCount = new IngredientCount();
            ingredientCount.SetBaseCount(cost.count);
            ingredientCount.filter.SetAllow(cost.thingDef, true);
            recipeDef.ingredients.Add(ingredientCount);
        }

        recipeDef.workSkill = SkillDefOf.Crafting;
        recipeDef.recipeUsers = [];
        (ThingDefOf.XFMParmor_FabricationPit.recipes ??
         (ThingDefOf.XFMParmor_FabricationPit.recipes = [])).Add(recipeDef);
        if (def.researchPrerequisites != null)
        {
            recipeDef.researchPrerequisites = def.researchPrerequisites;
        }

        recipeDef.defaultIngredientFilter = new ThingFilter();
        MechanicalArmorDef.recipes.Add(recipeDef, def);
        return recipeDef;
    }

    private static RecipeDef CreateRecipeDefFromModuleDef(ModuleDef def)
    {
        var recipeDef = new RecipeDef
        {
            defName = $"XFMParmor_InstallModule_{def.defName}",
            label = "XFMParmor_ModuleRecipeDef_label".Translate() + def.label,
            description = def.description,
            workerClass = typeof(RecipeWorker_InstallModule),
            jobString = "XFMParmor_ModuleRecipeDef_jobString".Translate(),
            workAmount = 600f,
            workSpeedStat = StatDefOf.WorkSpeedGlobal
        };
        foreach (var cost in def.costList)
        {
            var ingredientCount = new IngredientCount();
            ingredientCount.SetBaseCount(cost.count);
            ingredientCount.filter.SetAllow(cost.thingDef, true);
            recipeDef.ingredients.Add(ingredientCount);
        }

        recipeDef.workSkill = SkillDefOf.Crafting;
        recipeDef.recipeUsers = [];
        foreach (var value in MechanicalArmorDef.buildings.Values)
        {
            recipeDef.recipeUsers.Add(value);
        }

        recipeDef.defaultIngredientFilter = new ThingFilter();
        if (def.researchPrerequisites != null)
        {
            recipeDef.researchPrerequisites = def.researchPrerequisites;
        }

        ModuleDef.recipies.Add(recipeDef, def);
        return recipeDef;
    }
}