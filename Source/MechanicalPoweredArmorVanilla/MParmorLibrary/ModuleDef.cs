using System;
using System.Collections.Generic;
using Verse;

namespace MParmorLibrary;

public class ModuleDef : Def
{
    public static readonly Dictionary<RecipeDef, ModuleDef> recipies = new();

    public List<ThingDefCountClass> costList;
    public Type moduleClass;

    public List<ResearchProjectDef> researchPrerequisites;

    public override IEnumerable<string> ConfigErrors()
    {
        foreach (var item in base.ConfigErrors())
        {
            yield return item;
        }

        if (moduleClass == null)
        {
            yield return $"{GetType()} lacks moduleClass Label={label}";
        }
    }
}