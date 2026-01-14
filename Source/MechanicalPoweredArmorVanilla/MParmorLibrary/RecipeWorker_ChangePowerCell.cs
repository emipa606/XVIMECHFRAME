using System.Collections.Generic;
using RimWorld;
using Verse;

namespace MParmorLibrary;

public class RecipeWorker_ChangePowerCell : RecipeWorker_Maintain
{
    public override void Maintain(MParmorBuilding building, List<Thing> ingredients)
    {
        if (building.PowerTracker.IsFull)
        {
            ToolsLibrary.SpawnThingDefCount(GetIngredients(ingredients), building.Position, building.Map);
            MoteMaker.ThrowText(building.DrawPos, building.Map, "XFMParmor_RecipeWorker_ChangePowerCell".Translate());
        }
        else
        {
            building.PowerTracker.ChangeNewPowerCell();
        }
    }
}