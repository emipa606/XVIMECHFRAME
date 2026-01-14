using Verse;

namespace MParmorLibrary;

public class CompProperties_MParmorBuilding : CompProperties
{
    public MechanicalArmorDef mechanicalArmor;

    public CompProperties_MParmorBuilding()
    {
        compClass = typeof(CompMParmorBuilding);
    }
}