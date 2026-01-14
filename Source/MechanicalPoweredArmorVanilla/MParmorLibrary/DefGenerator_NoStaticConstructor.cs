using System;
using System.Collections.Generic;
using System.Reflection;
using RimWorld;
using UnityEngine;
using Verse;

namespace MParmorLibrary;

public static class DefGenerator_NoStaticConstructor
{
    public static void Invoke()
    {
        var named = DefDatabase<WorkGiverDef>.GetNamed("XFMParmor_WorkInMParmor");
        named.fixedBillGiverDefs = [];
        var list = new List<RecipeDef>();
        foreach (var item in DefDatabase<RecipeDef>.AllDefsListForReading)
        {
            if (item.Worker is RecipeWorker_Maintain)
            {
                list.Add(item);
            }
        }

        foreach (var building in MechanicalArmorDef.buildings)
        {
            var value = building.Value;
            value.label = building.Key.label + "XFMParmor_defNameBuildingFixed".Translate();
            value.description = building.Key.description;
            value.building.spawnedConceptLearnOpportunity = ConceptDefOf.BillsTab;
            value.comps.Add(new CompProperties_Usable
            {
                useJob = JobDefOf.XFMParmor_Job_GetIntoMParmor,
                useLabel = "XFMParmor_Job_GetIntoMParmor".Translate(),
                useDuration = 120
            });
            value.recipes = list;
            value.thingCategories = [ThingCategoryDef.Named("BuildingsSecurity")];
            value.interactionCellIcon = RimWorld.ThingDefOf.DiningChair;
            value.filthLeaving = RimWorld.ThingDefOf.Filth_RubbleBuilding;
            value.repairEffect = DefDatabase<EffecterDef>.GetNamed("Repair");
            value.terrainAffordanceNeeded = TerrainAffordanceDefOf.Light;
            value.soundImpactDefault = SoundDef.Named("BulletImpact_Metal");
            value.minifiedDef = RimWorld.ThingDefOf.MinifiedThing;
            named.fixedBillGiverDefs.Add(value);
            building.Key.building = value;
            var installBlueprintDef = value.installBlueprintDef;
            installBlueprintDef.label = value.label + "BlueprintLabelExtra".Translate();
            installBlueprintDef.graphicData.shaderType = ShaderTypeDefOf.EdgeDetect;
            typeof(GraphicData).GetMethod("Init", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.Invoke(installBlueprintDef.graphicData, []);
            building.Key.core.description = building.Key.description;
        }

        RecipeGenerator.GenerateImpliedDefs_PreResolve();
    }

    public static ThingDef NewBlueprintDef_Thing(ThingDef def)
    {
        if (typeof(ThingDefGenerator_Buildings)
                .GetMethod("BaseBlueprintDef", BindingFlags.Static | BindingFlags.NonPublic)
                ?.Invoke(null, []) is not ThingDef thingDef)
        {
            return null;
        }

        thingDef.defName = ThingDefGenerator_Buildings.BlueprintDefNamePrefix +
                           ThingDefGenerator_Buildings.InstallBlueprintDefNamePrefix + def.defName;
        thingDef.label = def.label;
        thingDef.size = def.size;
        thingDef.drawPlaceWorkersWhileSelected = def.drawPlaceWorkersWhileSelected;
        if (def.placeWorkers != null)
        {
            thingDef.placeWorkers = new List<Type>(def.placeWorkers);
        }

        thingDef.graphicData = new GraphicData();
        thingDef.graphicData.CopyFrom(def.graphicData);
        thingDef.graphicData.shaderType = null;
        thingDef.graphicData.color = new Color(0.8235294f, 47f / 51f, 1f, 0.6f);
        thingDef.graphicData.colorTwo = Color.white;
        thingDef.graphicData.shadowData = null;
        thingDef.graphicData.renderQueue = 2950;
        thingDef.thingClass = typeof(Blueprint_Install);
        thingDef.drawerType = def.drawerType;
        thingDef.entityDefToBuild = def;
        def.installBlueprintDef = thingDef;
        return thingDef;
    }
}