using System.Collections.Generic;
using RimWorld;
using Verse;

namespace MParmorLibrary;

public class RecipeWorker_FixMachine : RecipeWorker_Maintain
{
    public override void Maintain(MParmorBuilding building, List<Thing> ingredients)
    {
        if (building.HealthTracker.Machine == building.HealthTracker.MachineMax)
        {
            ToolsLibrary.SpawnThingDefCount(GetIngredients(ingredients), building.Position, building.Map);
            MoteMaker.ThrowText(building.DrawPos, building.Map, "XFMParmor_RecipeWorker_FixMachine".Translate());
        }
        else
        {
            building.HealthTracker.Machine += 1200f;
        }
    }
}