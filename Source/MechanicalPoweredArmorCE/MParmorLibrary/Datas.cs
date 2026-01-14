using System.Collections.Generic;
using Verse;

namespace MParmorLibrary;

public static class Datas
{
    public static bool CanIgnoreShield(this DamageInfo dinfo)
    {
        return dinfo.Weapon != null && DataLibraryInstance.DataLibrary.weaponDef_IgnoreShield.Contains(dinfo.Weapon);
    }

    public static List<string> GetForceFriendlyFire()
    {
        return ["鸦：护盾屏障，震撼冲击", "鹘：反弹子弹", "鸮：北极风", "机甲自爆伤害", "AOE近战"];
    }
}