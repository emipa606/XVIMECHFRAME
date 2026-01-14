using MParmorLibrary.SkillSystem;
using Verse;

namespace MParmorLibrary;

public class Verb_VigorouslyRebound : Verb
{
    public SkillObject Skill => DirectOwner as SkillObject;

    public MParmorCore_Red Core => Skill.parent.parent as MParmorCore_Red;

    protected override bool TryCastShot()
    {
        StartSkill();
        return true;
    }

    public void StartSkill()
    {
        Core.activeTime = (int)Skill.skill.skillValueA;
        Core.activeOutbreakUntilTick = Core.activeTime + Find.TickManager.TicksGame;
        Core.range = verbProps.range;
        Skill.UsedOnce();
    }
}