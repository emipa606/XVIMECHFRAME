using System.Collections.Generic;
using Verse;

namespace MParmorLibrary;

public class RecipeWorker_ReplenishSupply : RecipeWorker_Maintain
{
    public override bool AvailableOnNow(Thing thing, BodyPartRecord part = null)
    {
        if (thing is MParmorBuilding mParmorBuilding &&
            mParmorBuilding.ModulesTracker.TryGetModule<Module_Supply>("Module_Supply", out var module))
        {
            return module.Preparation < 14.400001f;
        }

        return false;
    }

    public override void Maintain(MParmorBuilding building, List<Thing> ingredients)
    {
        building.ModulesTracker.TryGetModule<Module_Supply>("Module_Supply", out var module);
        var stackCount = ingredients[0].stackCount;
        var preparation = module.Preparation;
        var num = 14.400001f - preparation;
        if (num >= stackCount)
        {
            module.Preparation += (stackCount * 1.6f) + 0.005f;
            return;
        }

        var num2 = (int)num;
        if (num > num2)
        {
            num2++;
        }

        module.Preparation = 14.400001f;
        stackCount -= num2;
        if (stackCount > 0)
        {
            ToolsLibrary.SpawnThingDefCount(new ThingDefCountClass(ingredients[0].def, stackCount), building.Position,
                building.Map);
        }
    }
}