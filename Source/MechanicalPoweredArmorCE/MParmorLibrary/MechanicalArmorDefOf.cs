using RimWorld;

namespace MParmorLibrary;

[DefOf]
public static class MechanicalArmorDefOf
{
    public static MechanicalArmorDef MechanicalArmor_Black;

    public static MechanicalArmorDef MechanicalArmor_Red;

    public static MechanicalArmorDef MechanicalArmor_Aqua;

    public static MechanicalArmorDef MechanicalArmor_Carrier;

    static MechanicalArmorDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(MechanicalArmorDefOf));
    }
}