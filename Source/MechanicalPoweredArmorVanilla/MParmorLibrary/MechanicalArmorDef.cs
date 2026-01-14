using System.Collections.Generic;
using System.Reflection;
using RimWorld;
using UnityEngine;
using Verse;

namespace MParmorLibrary;

public class MechanicalArmorDef : Def
{
    public static readonly Dictionary<MechanicalArmorDef, ThingDef> buildings =
        new Dictionary<MechanicalArmorDef, ThingDef>();

    public static readonly Dictionary<RecipeDef, MechanicalArmorDef> recipes =
        new Dictionary<RecipeDef, MechanicalArmorDef>();

    public readonly float moveSpeed = 4.5f;

    [Unsaved] public ThingDef building;

    public Color color = Color.white;

    public ThingDef core;

    public List<ThingDefCountClass> costList;

    public GraphicData frontGraphic;
    public float machine;

    public List<ResearchProjectDef> researchPrerequisites;

    public float shield;

    public int shieldBrokenCD;

    public float shieldRecovery;

    public int shieldRecoveryCD;

    public List<ThingDef> weapon;

    public ThingDef wreckage;

    public GraphicData wreckageGraphic;

    public override void PostLoad()
    {
        var thingDef = new ThingDef
        {
            defName = $"XFMParmor_Building_{defName}",
            label = "null",
            description = description,
            category = ThingCategory.Building,
            thingClass = typeof(MParmorBuilding),
            altitudeLayer = AltitudeLayer.Shadows,
            passability = Traversability.Standable,
            useHitPoints = false,
            selectable = true,
            drawerType = DrawerType.MapMeshAndRealTime,
            leaveResourcesWhenKilled = true,
            rotatable = false,
            tickerType = TickerType.Normal,
            size = new IntVec2(1, 1),
            graphicData = new GraphicData
            {
                graphicClass = typeof(Graphic_Single),
                texPath = $"{frontGraphic.texPath}_south",
                drawSize = new Vector2(3f, 3f)
            },
            hasInteractionCell = true,
            interactionCellOffset = IntVec3.Zero,
            interactionCellIconReverse = true,
            building = new BuildingProperties
            {
                buildingTags = ["Production"],
                isEdifice = false,
                spawnedConceptLearnOpportunity = null,
                unpoweredWorkTableWorkSpeedFactor = 1f,
                alwaysDeconstructible = false
            },
            comps =
            [
                new CompProperties_UseEffect
                {
                    compClass = typeof(CompUseEffect_GetIntoMParmor)
                },

                new CompProperties_MParmorBuilding
                {
                    mechanicalArmor = this
                }
            ],
            recipes = null,
            inspectorTabs = [typeof(ITab_Bills)],
            placeWorkers =
            [
                typeof(PlaceWorker_PreventInteractionSpotOverlap),
                typeof(PlaceWorker_ReportWorkSpeedPenalties)
            ],
            thingCategories = null,
            interactionCellIcon = null,
            filthLeaving = null,
            repairEffect = null,
            terrainAffordanceNeeded = null,
            soundImpactDefault = null,
            minifiedDef = null
        };
        typeof(BuildingProperties).GetField("deconstructible", BindingFlags.Instance | BindingFlags.NonPublic)
            ?.SetValue(thingDef.building, false);
        buildings.Add(this, thingDef);
        RimWorld.DefGenerator.AddImpliedDef(thingDef);
        var def = DefGenerator_NoStaticConstructor.NewBlueprintDef_Thing(thingDef);
        RimWorld.DefGenerator.AddImpliedDef(def);
    }
}