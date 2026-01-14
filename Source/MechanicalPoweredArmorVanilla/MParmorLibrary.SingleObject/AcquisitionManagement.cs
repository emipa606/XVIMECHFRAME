using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace MParmorLibrary.SingleObject;

public class AcquisitionManagement : IExposable
{
    public static AcquisitionManagement instance = new();

    public static bool instanceBoolForBodySize;

    public static bool instanceBoolForDrawPawnGUIOverlay;

    public List<MParmorCore> cores = [];

    public bool firstPast;

    public int firstTick = -1;

    public bool flag;

    public int randomDay;

    public int startTick;

    public static int MParmorCount =>
        ToolsLibrary_MParmorOnly.GetMParmor().Count + ToolsLibrary_MParmorOnly.GetMParmorBuilding().Count;

    public static float CountPercentage => (MParmorCount - 1) / 20f;

    public static int AntiTimeDay => (int)Mathf.Lerp(90f, 25f, CountPercentage);

    public int AntiTick => (AntiTimeDay + randomDay) * 60000;

    public void ExposeData()
    {
        Scribe_Values.Look(ref firstPast, "firstPast");
        Scribe_Values.Look(ref firstTick, "firstTick", -1);
        Scribe_Values.Look(ref startTick, "startTick");
        Scribe_Values.Look(ref randomDay, "randomDay");
        Scribe_Collections.Look(ref cores, "cores", LookMode.Reference);
        if (Scribe.mode == LoadSaveMode.LoadingVars)
        {
            ResetCachePool();
        }
    }

    public static AcquisitionManagement GetInstance()
    {
        return instance ?? (instance = new AcquisitionManagement());
    }

    public void Tick()
    {
        if (DefDatabase<RecipeDef>.GetNamed("XFMParmor_Fabricate_MechanicalArmor_Black", false) == null && !flag)
        {
            DefGenerator_NoStaticConstructor.Invoke();
            flag = true;
        }

        FirstCore();
        if (MParmorCount <= 0)
        {
            return;
        }

        startTick++;
        if (startTick > AntiTick)
        {
            Anti();
        }
    }

    private void Anti()
    {
        var list = Find.Maps.FindAll(x => x.IsPlayerHome);
        var target = list[Rand.Range(0, list.Count)];
        var xFMParmor_RaidEnemy_AntiMPArmor = IncidentDefOf.XFMParmor_RaidEnemy_AntiMPArmor;
        var incidentParms = StorytellerUtility.DefaultParmsNow(xFMParmor_RaidEnemy_AntiMPArmor.category, target);
        incidentParms.points *= 0.3f;
        incidentParms.points += MParmorCount * 2300;
        incidentParms.forced = true;
        if (xFMParmor_RaidEnemy_AntiMPArmor.Worker.CanFireNow(incidentParms))
        {
            xFMParmor_RaidEnemy_AntiMPArmor.Worker.TryExecute(incidentParms);
            startTick = 0;
            randomDay = Rand.Range(-1, 3);
        }
        else
        {
            startTick = (int)(startTick * 0.8f);
        }
    }

    private void FirstCore()
    {
        if (firstTick > 0)
        {
            firstTick--;
        }
        else if (!firstPast && ResearchProjectDefOf.XFMParmor_Root.IsFinished)
        {
            var list = Find.Maps.FindAll(x => x.IsPlayerHome);
            var target = list[Rand.Range(0, list.Count)];
            var parms = new IncidentParms
            {
                target = target,
                points = StorytellerUtility.DefaultThreatPointsNow(target)
            };
            if (firstTick == -1)
            {
                var num = Rand.Range(4, 13);
                var num2 = Rand.Range(-2000, 2001);
                firstTick = (num * 60000) + (num2 * 10);
                ToolsLibrary.SendStandardLetter("XFMParmor_AcquisitionManagement_A".Translate(),
                    "XFMParmor_AcquisitionManagement_B".Translate(num), LetterDefOf.PositiveEvent, parms,
                    LookTargets.Invalid);
            }
            else if (IncidentDefOf.MParmor_WreckageFall.Worker.CanFireNow(parms) &&
                     IncidentDefOf.MParmor_WreckageFall.Worker.TryExecute(parms))
            {
                firstPast = true;
            }
        }
    }

    public bool IncidentA(IncidentParms parms)
    {
        return IncidentDefOf.XFMParmor_GiveQuest_SleepingMechanoids.Worker.TryExecute(parms);
    }

    private static void ResetCachePool()
    {
        MParmorBuilding.Cache = [];
        Building_FabricationCentralSystem.Cache = [];
        Building_FabricationPit.Cache = [];
        Wall_DescendingWall.instances = [];
        ProjectilesStore.GetInstance().projectiles = [];
        Intercepts.intercepts.Clear();
    }
}