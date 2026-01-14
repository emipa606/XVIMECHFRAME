using System.Collections.Generic;
using Verse;

namespace MParmorLibrary;

public abstract class RecipeWorker_Maintain : RecipeWorker
{
    public abstract void Maintain(MParmorBuilding building, List<Thing> ingredients);

    public static IEnumerable<ThingDefCountClass> GetIngredients(List<Thing> ingredients)
    {
        foreach (var x in ingredients)
        {
            yield return new ThingDefCountClass(x.def, x.stackCount);
        }
    }
}