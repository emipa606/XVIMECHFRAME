using System.Collections.Generic;
using System.Reflection;
using CombatExtended;
using MParmorLibrary.Settings;
using MParmorLibrary.SingleObject;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace MParmorLibrary;

public class MParmorCore : Apparel, IMParmorInstance, IAntiSuppressable
{
    private MParmorT_AITracker aiTracker;

    private MParmorT_HealthTracker healthTracker;

    public bool isDraftBefore = true;
    public bool isNaturallySpawned = true;

    private MParmorT_ModulesTracker modulesTracker;

    private MParmorT_PowerTracker powerTracker;

    public MParmorT_AITracker AiTracker
    {
        get
        {
            if (aiTracker != null)
            {
                return aiTracker;
            }

            return aiTracker = MParmorT_AITracker.CreateNewTracker(this);
        }
    }

    public Thing ThingFixed
    {
        get
        {
            if (Wearer.Spawned)
            {
                return Wearer;
            }

            if (Wearer.ParentHolder is Thing { Spawned: not false } thing)
            {
                return thing;
            }

            return null;
        }
    }

    public IntVec3 PositionFixed => ThingFixed?.Position ?? IntVec3.Zero;

    public virtual float MoveSpeed => PowerTracker.IsLow ? 4f : System.moveSpeed;

    public virtual bool CanChargeSkills => !PowerTracker.IsLow;

    public virtual bool CanChargeShield => !PowerTracker.IsLow;

    public virtual bool CanAntiSuppressable => true;

    public MechanicalArmorDef System => def.GetCompProperties<CompProperties_MechanicalArmor>().mechanicalArmor;

    public MParmorT_PowerTracker PowerTracker => powerTracker ?? (powerTracker = new MParmorT_PowerTracker(this));

    public MParmorT_HealthTracker HealthTracker => healthTracker ?? (healthTracker = new MParmorT_HealthTracker(this));

    public MParmorT_ModulesTracker ModulesTracker =>
        modulesTracker ?? (modulesTracker = new MParmorT_ModulesTracker(this));

    public void CopyTracker(IMParmorInstance instance)
    {
        powerTracker = instance.PowerTracker;
        powerTracker.instance = this;
        healthTracker = instance.HealthTracker;
        healthTracker.instance = this;
        modulesTracker = instance.ModulesTracker;
        modulesTracker.instance = this;
    }

    public virtual void ReloadAmmoForPrimary()
    {
        var val = Wearer.equipment?.Primary?.GetComp<CompAmmoUser>();
        val?.CurMagCount = val.MagSize;
    }

    public void AbsorbDamage(DamageInfo dinfo)
    {
        if (!PreTakeDamage(dinfo))
        {
            HealthTracker.TakeDamage(dinfo);
        }
    }

    protected virtual bool PreTakeDamage(DamageInfo dinfo)
    {
        return false;
    }

    protected sealed override void Tick()
    {
        if (isNaturallySpawned || !Wearer.Faction.IsPlayer)
        {
            var wearer = Wearer;
            AcquisitionManagement.GetInstance().cores.Remove(this);
            wearer.equipment.Remove(Wearer.equipment.Primary);
            wearer.apparel.Remove(this);
            return;
        }

        base.Tick();
        if (Wearer is not { Spawned: true })
        {
            return;
        }

        DraftTick();
        TickMParmor(out var returnNow);
        if (returnNow)
        {
            return;
        }

        HealthTracker.Tick();
        ModulesTracker.TickCore();
        if (!PowerTracker.Tick(Wearer))
        {
            MoteMaker.ThrowText(Wearer.DrawPos, Wearer.Map, "XFMParmor_MParmorCore_PowerTracker".Translate());
            GetOutOfMParmorForce();
        }
        else if (!StateControl() && AiTracker.IsActive)
        {
            AiTracker.Tick();
        }
    }

    protected virtual void TickMParmor(out bool returnNow)
    {
        returnNow = false;
    }

    public bool StateControl()
    {
        if (Wearer.InMentalState)
        {
            if (Wearer.MentalState.causedByPsycast || Wearer.MentalState.causedByDamage)
            {
                MoteMaker.ThrowText(Wearer.DrawPos, Wearer.Map, "XFMParmor_MParmorCore_StateControlA".Translate());
                Wearer.MentalState.RecoverFromState();
                return false;
            }

            MoteMaker.ThrowText(Wearer.DrawPos, Wearer.Map, "XFMParmor_MParmorCore_StateControlB".Translate());
            GetOutOfMParmorForce();
            return true;
        }

        if (!Wearer.Dead && !Wearer.Downed)
        {
            return false;
        }

        GetOutOfMParmorForce();
        return true;
    }

    public void DraftTick()
    {
        if (Setting.Settings.forceDrafted && Find.TickManager.TicksGame % 60 == 0)
        {
            var obj = typeof(Pawn_DraftController)
                .GetField("autoUndrafter", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.GetValue(Wearer.drafter) as AutoUndrafter;
            typeof(AutoUndrafter).GetField("lastNonWaitingTick", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.SetValue(obj, Find.TickManager.TicksGame);
        }

        if (!Wearer.Drafted && isDraftBefore)
        {
            if (Setting.Settings.getOutAfterDrafted && !Wearer.IsFormingCaravan())
            {
                var thing = GenClosest.ClosestThingReachable(Wearer.Position, Wearer.Map,
                    ThingRequest.ForDef(ThingDefOf.XFMParmor_FabricationPit), PathEndMode.OnCell,
                    TraverseParms.For(Wearer), 9999f, validator);
                if (thing != null)
                {
                    var job = JobMaker.MakeJob(JobDefOf.XFMParmor_Job_ReturnToPit, thing, -1, true);
                    Wearer.jobs.TryTakeOrderedJob(job, JobTag.Misc);
                    goto IL_0186;
                }
            }

            Messages.Message("XFMParmor_MParmorCore_DraftTick".Translate(Wearer.LabelCap), Wearer,
                MessageTypeDefOf.RejectInput, false);
            goto IL_0186;
        }

        if (!isDraftBefore && Wearer.Drafted)
        {
            isDraftBefore = true;
        }

        return;
        IL_0186:
        isDraftBefore = false;
        return;

        bool validator(Thing x)
        {
            return x is Building_FabricationPit { State: FabricationState.Free } && !x.IsForbidden(Wearer) &&
                   Wearer.CanReserve(x);
        }
    }

    public void GetOutOfMParmor()
    {
        if (Wearer.Position.GetFirstThing(Wearer.Map, ThingDefOf.XFMParmor_FabricationPit) is Building_FabricationPit
            {
                State: not FabricationState.Free
            })
        {
            Messages.Message("XFMParmor_MParmorCore_GetOutOfMParmor".Translate(), Wearer, MessageTypeDefOf.RejectInput,
                false);
        }
        else
        {
            GetOutOfMParmorForce();
        }
    }

    private void GetOutOfMParmorForce()
    {
        var mParmorBuilding = ThingMaker.MakeThing(System.building) as MParmorBuilding;
        mParmorBuilding?.CopyTracker(this);
        mParmorBuilding?.SetFactionDirect(Wearer.Faction);
        GenPlace.TryPlaceThing(mParmorBuilding, Wearer.Position, Wearer.Map, ThingPlaceMode.Direct, null, null,
            default(Rot4));
        var wearer = Wearer;
        AcquisitionManagement.GetInstance().cores.Remove(this);
        wearer.equipment.Remove(Wearer.equipment.Primary);
        wearer.apparel.Remove(this);
    }

    public void SelfDestruct()
    {
        Messages.Message("XFMParmor_MParmorCore_SelfDestruct".Translate(Wearer.LabelCap), Wearer,
            MessageTypeDefOf.RejectInput, false);
        SoundDefOf.XFMParmor_DestroyMParmor.PlayOneShot(new TargetInfo(Wearer.Position, Wearer.Map));
        var mParmorSelfDestruct =
            ThingMaker.MakeThing(ThingDefOf.XFMParmor_MechanicalArmorCore_Wreckage) as MParmorSelfDestruct;
        mParmorSelfDestruct?.system = System;
        mParmorSelfDestruct?.energy = (int)(PowerTracker.Battery / 30f);
        mParmorSelfDestruct?.Restart();
        var wearer = Wearer;
        Wearer.equipment.Remove(Wearer.equipment.Primary);
        AcquisitionManagement.GetInstance().cores.Remove(this);
        wearer.apparel.Remove(this);
        wearer.apparel.Wear(mParmorSelfDestruct, true, true);
    }

    public sealed override void DrawWornExtras()
    {
        if (isNaturallySpawned)
        {
            var wearer = Wearer;
            AcquisitionManagement.GetInstance().cores.Remove(this);
            wearer.equipment.Remove(Wearer.equipment.Primary);
            wearer.apparel.Remove(this);
        }
        else
        {
            var wearer2 = Wearer;
            if (wearer2 is { Spawned: true })
            {
                MParmorDrawAt(Wearer.Drawer.DrawPos);
            }
        }
    }

    public void MParmorDrawAt(Vector3 vector3)
    {
        BaseMParmorDrawAt(vector3);
        ModulesTracker.DrawCore(vector3);
    }

    protected virtual void BaseMParmorDrawAt(Vector3 vector3)
    {
        var loc = vector3;
        loc.y += 0.037744787f;
        System.frontGraphic.Graphic.Draw(loc, Wearer.Rotation, Wearer);
    }

    public override IEnumerable<Gizmo> GetWornGizmos()
    {
        foreach (var wornGizmo in base.GetWornGizmos())
        {
            yield return wornGizmo;
        }

        var gizmoAI = AiTracker.GetSwitchGizmo();
        if (gizmoAI != null)
        {
            yield return gizmoAI;
        }

        if (Find.Selector.SingleSelectedThing != Wearer)
        {
            yield break;
        }

        yield return new Gizmo_MParmorGUI
        {
            core = this,
            healthTracker = HealthTracker,
            powerTracker = PowerTracker
        };
        yield return new Command_Action
        {
            Order = 10f,
            defaultLabel = "XFMParmor_MParmorCore_Command_Action".Translate(),
            icon = Texture2DOf.GetOut,
            action = GetOutOfMParmor
        };
    }

    public virtual IEnumerable<Gizmo> GetExtraSkillGizmos()
    {
        yield break;
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Deep.Look(ref powerTracker, "powerTracker", this);
        Scribe_Deep.Look(ref healthTracker, "healthTracker", this);
        Scribe_Deep.Look(ref modulesTracker, "modulesTracker", this);
        Scribe_Values.Look(ref isNaturallySpawned, "isNaturallySpawned");
        if (Scribe.mode == LoadSaveMode.LoadingVars)
        {
            aiTracker = MParmorT_AITracker.CreateNewTracker(this);
        }

        aiTracker?.PostExposeData();
    }
}