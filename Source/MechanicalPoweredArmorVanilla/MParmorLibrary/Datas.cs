using Verse;

namespace MParmorLibrary;

public static class Datas
{
    public static bool CanIgnoreShield(this DamageInfo dinfo)
    {
        return dinfo.Weapon != null && DataLibraryInstance.DataLibrary.weaponDef_IgnoreShield.Contains(dinfo.Weapon);
    }
}