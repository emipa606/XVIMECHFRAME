using System;
using Verse;

namespace MParmorLibrary;

public class CompProperties_MechanicalArmor : CompProperties
{
    [MustTranslate] public readonly string AiText = "";

    public Type AiClass;
    public MechanicalArmorDef mechanicalArmor;

    public CompProperties_MechanicalArmor()
    {
        compClass = typeof(CompMechanicalArmor);
    }
}