using System.Collections.Generic;
using Verse;

namespace MParmorLibrary.SkillSystem;

public class CompProperties_Skills : CompProperties
{
    public readonly List<Skill> skills = [];

    public CompProperties_Skills()
    {
        compClass = typeof(CompSkills);
    }
}