using System.Collections.Generic;
using RimWorld;

namespace MParmorLibrary;

public class Verb_MeleeAttact_ChangingSpeed : Verb_MeleeAttackDamage
{
    private MParmorCore_Red Core
    {
        get
        {
            if (field != null)
            {
                return field;
            }

            IEnumerable<Apparel> enumerable = CasterPawn.apparel?.WornApparel;
            foreach (var item in enumerable ?? [])
            {
                if (item is MParmorCore_Red mParmorCore_Red)
                {
                    return field = mParmorCore_Red;
                }
            }

            return null;
        }
    }

    protected override bool TryCastShot()
    {
        tool.cooldownTime = MParmorCore_Red.AttactSpeedTime(Core.attactSpeedLevel) / 60f;
        Core.AttactSpeedLevelUp(Core.attactSpeedLevel);
        return base.TryCastShot();
    }
}