using System.Collections.Generic;
using Verse;

namespace MParmorLibrary;

public class Health_Shiled : IExposable
{
    private float damageShieldTotal;

    private float shieldInt = -1f;
    private float shieldMax = 100f;

    // ReSharper disable once EmptyConstructor
    // Explicit parameterless constructor required for Scribe serialization
    public Health_Shiled()
    {
    }

    public float ShieldMax
    {
        get => shieldMax;
        set => shieldMax = value;
    }

    public float Shield
    {
        get => shieldInt == -1f ? shieldInt = shieldMax : shieldInt;
        private set
        {
            if (value > ShieldMax)
            {
                shieldInt = ShieldMax;
            }
            else if (value < 0f)
            {
                shieldInt = 0f;
            }
            else
            {
                shieldInt = value;
            }
        }
    }

    public void ExposeData()
    {
        Scribe_Values.Look(ref shieldInt, "sheild", -1f);
        Scribe_Values.Look(ref shieldMax, "shieldMax", -1f);
    }

    public void Tick()
    {
        if (damageShieldTotal > 0f)
        {
            damageShieldTotal = DamageValueChangeSecond(damageShieldTotal, ShieldMax);
        }
    }

    private static float DamageValueChangeSecond(float value, float max)
    {
        var num = value / max;
        num = num - 0.02f > num * 0.9f ? num - 0.02f : num * 0.9f;
        if (num < 0.005f)
        {
            return 0f;
        }

        return max * num;
    }

    public void Hurt_Shield(DamageInfo dinfo)
    {
        if (!dinfo.Def.harmsHealth)
        {
            return;
        }

        Shield -= dinfo.Amount;
        damageShieldTotal += dinfo.Amount > Shield ? Shield : dinfo.Amount;
    }

    public IEnumerable<Gizmo> GetGizmos(string label, float fillPercent = -1f)
    {
        yield return new Gizmo_Shield
        {
            text = label,
            text2 = $"{Shield:0.#}/{ShieldMax:0}",
            fillPercent = (damageShieldTotal + shieldInt) / ShieldMax,
            fillPercent2 = Shield / ShieldMax,
            fillPercent3 = fillPercent
        };
        if (Prefs.DevMode)
        {
            yield return new Command_Action
            {
                Order = 1000f,
                defaultLabel = "Debug:Reset shield",
                action = delegate { Shield = ShieldMax; }
            };
        }
    }
}