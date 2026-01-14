using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace MParmorLibrary;

public class RecipeWorker_InstallModule : RecipeWorker_Maintain
{
    public override bool AvailableOnNow(Thing thing, BodyPartRecord part = null)
    {
        if (thing is not MParmorBuilding mParmorBuilding)
        {
            return false;
        }

        var module = ModuleDef.recipies.FirstOrDefault(pair => pair.Key.defName == recipe.defName).Value;
        if (module == null)
        {
            return false;
        }

        return !mParmorBuilding.ModulesTracker.TryGetModule(module.defName, out Module _);
    }

    public override void Maintain(MParmorBuilding building, List<Thing> ingredients)
    {
        var moduleDef = ModuleDef.recipies.FirstOrDefault(pair => pair.Key.defName == recipe.defName).Value;
        if (moduleDef == null)
        {
            return;
        }

        if (building.ModulesTracker.TryLoadNewModule(moduleDef))
        {
            return;
        }

        ToolsLibrary.SpawnThingDefCount(GetIngredients(ingredients), building.Position, building.Map);
        MoteMaker.ThrowText(building.DrawPos, building.Map,
            "XFMParmor_RecipeWorker_InstallModule".Translate(moduleDef.label));
    }
}