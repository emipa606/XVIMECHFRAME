using Verse;

namespace MParmorLibrary.SkillSystem;

public class Skill
{
    public readonly bool chargeByTime = true;

    public readonly bool noNeedCharge = false;

    public string description;

    public int energy;

    [NoTranslate] public string id;

    public string label;

    public float skillValueA;

    public float skillValueB;

    public float skillValueC;
    public VerbProperties verb;
}