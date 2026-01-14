using System;
using RimWorld;
using Verse;
using Verse.Sound;

namespace MParmorLibrary;

public class MParmorT_HealthTracker(IMParmorInstance instance) : IExposable
{
    private float damageMachineTotal;

    private float damageShieldTotal;

    public IMParmorInstance instance = instance;

    private float machineInt = -1f;

    private float shieldInt = -1f;

    private int shieldRecoveryCDInt;

    private int superShieldTime;

    public Thing Thing
    {
        get
        {
            if (instance is MParmorCore mParmorCore)
            {
                return mParmorCore.Wearer;
            }

            return instance as Thing;
        }
    }

    public MechanicalArmorDef System => instance.System;

    public float MachineMax => System.machine;

    public float Machine
    {
        get => machineInt == -1f ? machineInt = MachineMax : machineInt;
        set
        {
            machineInt = value;
            if (machineInt < 0f)
            {
                machineInt = 0f;
            }

            if (machineInt > MachineMax)
            {
                machineInt = MachineMax;
            }
        }
    }

    public float ShieldMax => System.shield;

    public float Shield
    {
        get => shieldInt == -1f ? shieldInt = 0f : shieldInt;
        private set
        {
            shieldInt = value;
            if (shieldInt < 0f)
            {
                shieldInt = 0f;
            }

            if (shieldInt > ShieldMax)
            {
                shieldInt = ShieldMax;
            }
        }
    }

    public int ShieldRecoveryCD => System.shieldRecoveryCD;

    public int ShieldBrokenCD => System.shieldBrokenCD;

    public float ShieldRecovery => System.shieldRecovery;

    public int ShieldRecoveryCDInt => shieldRecoveryCDInt;

    public float DamageMachineTotal => damageMachineTotal;

    public float DamageShieldTotal => damageShieldTotal;

    public int HurtMachine { get; private set; }

    public int HurtShield { get; private set; }

    public int SuperShieldTime => superShieldTime;

    public void ExposeData()
    {
        Scribe_Values.Look(ref machineInt, "machineInt");
        Scribe_Values.Look(ref shieldInt, "shieldInt");
        Scribe_Values.Look(ref shieldRecoveryCDInt, "shieldRecoveryCDInt");
        Scribe_Values.Look(ref superShieldTime, "superShieldTime");
    }

    public void TakeDamage(DamageInfo dinfo)
    {
        if (!dinfo.Def.harmsHealth || dinfo.Instigator == Thing)
        {
            return;
        }

        if (Thing is Pawn pawn)
        {
            pawn.mindState.Notify_DamageTaken(dinfo);
            pawn.mindState.lastHarmTick = Find.TickManager.TicksGame;
        }

        if (Shield > 0f && !dinfo.CanIgnoreShield())
        {
            Hurt_Shield(dinfo);
        }
        else
        {
            Hurt_Machine(dinfo);
        }
    }

    private static float DamageValueChangeSecond(ref float value, float max)
    {
        var num = value / max;
        num = num - 0.02f > num * 0.9f ? num - 0.02f : num * 0.9f;
        if (!(num < 0.005f))
        {
            return max * num;
        }

        value = 0f;
        return 0f;
    }

    public void Hurt_Shield(DamageInfo dinfo)
    {
        if (dinfo.Weapon != null && DataLibraryInstance.DataLibrary.weaponDef_AntiShield.Contains(dinfo.Weapon))
        {
            dinfo.SetAmount(dinfo.Amount * 3f);
        }

        if (instance is MParmorCore mParmorCore && mParmorCore.AiTracker.IsActive)
        {
            mParmorCore.AiTracker.GetHurt_Shield(dinfo);
        }

        HurtShield = 50;
        if (SuperShieldTime > 0 && (dinfo.Instigator == null || dinfo.Instigator.Faction == null ||
                                    dinfo.Instigator.Faction != Thing.Faction))
        {
            Shield += dinfo.Amount;
        }

        damageShieldTotal += dinfo.Amount > Shield ? Shield : dinfo.Amount;
        Shield -= dinfo.Amount;
        shieldRecoveryCDInt = ShieldRecoveryCDInt > ShieldRecoveryCD ? ShieldRecoveryCDInt : ShieldRecoveryCD;
        if (Shield == 0f)
        {
            SheildBreak();
            return;
        }

        RimWorld.SoundDefOf.EnergyShield_AbsorbDamage.PlayOneShot(new TargetInfo(Thing.Position, Thing.Map));
        var loc = Thing.TrueCenter() + (Vector3Utility.HorizontalVectorFromAngle(dinfo.Angle).RotatedBy(180f) * 0.5f);
        var num = Math.Max(Rand.Range(dinfo.Amount / 70f, dinfo.Amount / 10f), 1f);
        FleckMaker.Static(loc, Thing.Map, RimWorld.FleckDefOf.ExplosionFlash, num);
        var num2 = (int)num;
        for (var i = 0; i < num2; i++)
        {
            FleckMaker.ThrowDustPuff(loc, Thing.Map, Rand.Range(0.8f, 1.2f));
        }
    }

    public void Tick()
    {
        if (DamageMachineTotal > 0f)
        {
            damageMachineTotal = DamageValueChangeSecond(ref damageMachineTotal, MachineMax);
        }

        if (DamageShieldTotal > 0f)
        {
            damageShieldTotal = DamageValueChangeSecond(ref damageShieldTotal, ShieldMax);
        }

        if (HurtMachine > 0)
        {
            HurtMachine--;
        }

        if (HurtShield > 0)
        {
            HurtShield--;
        }

        RecoveryShieldTick();
        if (SuperShieldTime > 0)
        {
            superShieldTime--;
        }
    }

    private void RecoveryShieldTick()
    {
        if (instance is MParmorCore { CanChargeShield: not false })
        {
            if (ShieldRecoveryCDInt > 0)
            {
                shieldRecoveryCDInt--;
            }
            else if (Shield < ShieldMax)
            {
                RecoverShield(ShieldRecovery, true);
            }
        }
        else
        {
            shieldRecoveryCDInt = ShieldRecoveryCDInt > ShieldRecoveryCD ? ShieldRecoveryCDInt : ShieldRecoveryCD;
        }
    }

    private void RecoverShield(float amount, bool consumingPower = false)
    {
        if (consumingPower)
        {
            var num = ShieldMax - Shield;
            if (amount > num)
            {
                instance.PowerTracker.ConsumeBatteryExactly((int)(num * 250f));
            }

            instance.PowerTracker.ConsumeBatteryExactly((int)(amount * 250f));
        }

        Shield += amount;
        damageShieldTotal -= amount;
        if (DamageShieldTotal < 0f)
        {
            damageShieldTotal = 0f;
        }
    }

    public void SheildBreak()
    {
        shieldRecoveryCDInt = ShieldBrokenCD;
    }

    public void Hurt_Machine(DamageInfo dinfo)
    {
        if (dinfo.Weapon != null && DataLibraryInstance.DataLibrary.weaponDef_AntiMachine.Contains(dinfo.Weapon))
        {
            dinfo.SetAmount(dinfo.Amount * 3f);
        }

        if (instance is MParmorCore mParmorCore && mParmorCore.AiTracker.IsActive)
        {
            mParmorCore.AiTracker.GetHurt_Machine(dinfo);
        }

        HurtMachine = 50;
        damageMachineTotal += dinfo.Amount > Machine ? Machine : dinfo.Amount;
        Machine -= dinfo.Amount;
        shieldRecoveryCDInt = ShieldRecoveryCDInt > ShieldRecoveryCD ? ShieldRecoveryCDInt : ShieldRecoveryCD;
        SoundDefOf.XFMParmor_HitMachine.PlayOneShot(new TargetInfo(Thing.Position, Thing.Map));
        if (Machine == 0f)
        {
            (instance as MParmorCore)?.SelfDestruct();
        }
    }
}