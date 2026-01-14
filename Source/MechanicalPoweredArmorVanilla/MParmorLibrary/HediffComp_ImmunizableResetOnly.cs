using RimWorld;
using UnityEngine;
using Verse;

namespace MParmorLibrary;

public class HediffComp_ImmunizableResetOnly : HediffComp_Immunizable
{
    public override string CompLabelInBracketsExtra =>
        Mathf.RoundToInt(parent.Severity / Mathf.Abs(Props.severityPerDayNotImmune) * 24f).ToString() +
        "LetterHour".Translate();

    public override float SeverityChangePerDay()
    {
        return Pawn.jobs.posture == PawnPosture.Standing ? 0f : Props.severityPerDayNotImmune;
    }
}