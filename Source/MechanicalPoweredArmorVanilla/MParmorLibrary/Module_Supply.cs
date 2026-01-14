using System;
using RimWorld;
using Verse;

namespace MParmorLibrary;

public class Module_Supply : Module
{
    public const float preparationPerDay = 1.6f;

    private const int preparationMaxDay = 9;

    public const float preparationMax = 14.400001f;
    private float preparation;

    public float Preparation
    {
        get => preparation;
        set
        {
            preparation = value;
            if (preparation > 14.400001f)
            {
                preparation = 14.400001f;
            }

            if (preparation < 0f)
            {
                preparation = 0f;
            }
        }
    }

    public float PreparationPercentage => Preparation / 14.400001f;

    public float PreparationByDay
    {
        get
        {
            if (Preparation == 14.400001f)
            {
                return 9f;
            }

            return Preparation / 1.6f;
        }
    }

    public override string TextOnGizmo => "XFMParmor_Module_Supply".Translate(PreparationByDay.ToString("0.#"));

    public override void TickCore()
    {
        if (Find.TickManager.TicksGame % 180 != 0)
        {
            return;
        }

        var wearer = Core.Wearer;
        var needs = Core.Wearer.needs;
        if (needs.rest.CurCategory == RestCategory.Rested && needs.food.CurCategory == HungerCategory.Fed)
        {
            return;
        }

        var num = Math.Max(needs.food.MaxLevel - needs.food.CurLevel,
            (needs.rest.MaxLevel - needs.rest.CurLevel) * 1.6f);
        var curLevel = needs.rest.CurLevel;
        if (Preparation > num)
        {
            needs.rest.CurLevel += num / 1.6f;
            needs.food.CurLevel += num;
            Preparation -= num;
            HealthUtility.AdjustSeverity(wearer, HediffDefOf.XFMParmor_Weariness,
                (needs.rest.CurLevel - curLevel) * 8f);
        }
        else
        {
            needs.rest.CurLevel += Preparation / 1.6f;
            needs.food.CurLevel += Preparation;
            HealthUtility.AdjustSeverity(wearer, HediffDefOf.XFMParmor_Weariness,
                (needs.rest.CurLevel - curLevel) * 8f);
            Preparation = 0f;
        }
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref preparation, "preparation");
    }
}