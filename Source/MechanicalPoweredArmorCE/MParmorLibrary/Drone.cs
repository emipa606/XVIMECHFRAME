using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CombatExtended;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace MParmorLibrary;

public class Drone : Pawn, IIntercept
{
    public LocalTargetInfo forceTarget;

    public bool isAttactMode;
    public MParmorCore origin;

    public bool searthNewTarget = true;

    private Health_Shiled shieldClass;

    public int spawnTick;

    public Health_Shiled ShieldClass
    {
        get
        {
            shieldClass ??= new Health_Shiled();

            return shieldClass;
        }
        set => shieldClass = value;
    }

    public bool CanIntercept(Projectile projectile, IntVec3 c)
    {
        var num = (this.TrueCenter() - projectile.ExactPosition).MagnitudeHorizontalSquared();
        return ToolsLibrary_MParmorOnly.IsUnfriendly(projectile, this) && num < 24.01f && num > 12.25;
    }

    public bool TryIntercept(Projectile projectile, IntVec3 c)
    {
        var dinfo = new DamageInfo(projectile.def.projectile.damageDef, projectile.DamageAmount,
            projectile.ArmorPenetration, projectile.ExactRotation.eulerAngles.y, projectile.Launcher, null,
            projectile.EquipmentDef, DamageInfo.SourceCategory.ThingOrUnknown, projectile.intendedTarget.Thing);
        ShowImpectFleck(c.ToVector3Shifted(), dinfo.Amount);
        ShieldClass.Hurt_Shield(dinfo);
        projectile.GetType().GetMethod("Impact", BindingFlags.Instance | BindingFlags.NonPublic)
            ?.Invoke(projectile, new object[1]);
        if (projectile.Spawned)
        {
            projectile.Destroy();
        }

        return true;
    }

    public bool CanIntercept(ProjectileCE projectile, IntVec3 c)
    {
        float num = (Position - c).LengthHorizontalSquared;
        return ToolsLibrary_MParmorOnly.IsUnfriendly(projectile, this) && num is < 29.160002f and > 12.25f;
    }

    public bool TryIntercept(ProjectileCE projectile, IntVec3 c)
    {
        var num = 0f;
        foreach (var item in projectile.GetProjectileDamageInfo())
        {
            num += item.Amount;
            ShieldClass.Hurt_Shield(item);
        }

        ShowImpectFleck(c.ToVector3Shifted(), num);
        projectile.Impact(null);
        if (projectile.Spawned)
        {
            projectile.Destroy();
        }

        return true;
    }

    private static Drone MakeNewDrone(MParmorCore origin)
    {
        var request = new PawnGenerationRequest(PawnKindDefOf.XFMParmor_Drone_Carrier, origin.Wearer.Faction,
            PawnGenerationContext.NonPlayer, -1);
        var drone = PawnGenerator.GeneratePawn(request) as Drone;
        drone?.origin = origin;
        drone?.spawnTick = Find.TickManager.TicksGame;
        return drone;
    }

    public static void SpawnDroneToDefendTarget(MParmorCore origin, LocalTargetInfo target)
    {
        var drone = MakeNewDrone(origin);
        drone.forceTarget = target;
        drone.ShieldClass.ShieldMax = 2700f;
        GenSpawn.Spawn(drone, origin.PositionFixed, origin.ThingFixed.Map);
        Intercepts.AddNewInstance(drone);
        foreach (var item in from x in drone.health.hediffSet.GetNotMissingParts()
                 where x.def.defName == "Blade"
                 select x)
        {
            var hediff = HediffMaker.MakeHediff(HediffDefOf.XFMParmor_MissingBodyPart_NotInstalled_Blade, drone, item);
            drone.health.hediffSet.AddDirect(hediff);
        }
    }

    public static void SpawnDroneToMeleeTarget(MParmorCore origin, LocalTargetInfo target)
    {
        var drone = MakeNewDrone(origin);
        drone.isAttactMode = true;
        drone.ShieldClass.ShieldMax = 200f;
        GenSpawn.Spawn(drone, origin.PositionFixed, origin.ThingFixed.Map);
        if (target.HasThing)
        {
            drone.SetMeleeTarget(target.Thing);
        }
        else
        {
            var job = JobMaker.MakeJob(RimWorld.JobDefOf.Goto, target.Cell, -1, true);
            drone.jobs.TryTakeOrderedJob(job, JobTag.Misc);
        }
    }

    public override void PreApplyDamage(ref DamageInfo dinfo, out bool absorbed)
    {
        absorbed = true;
        ShieldClass.Hurt_Shield(dinfo);
    }

    public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
    {
        Intercepts.RemoveInstance(this);
        base.DeSpawn(mode);
    }

    protected override void Tick()
    {
        base.Tick();
        if (origin == null || origin.Wearer == null)
        {
            Destroy();
            return;
        }

        if (GetBattery() <= 0f)
        {
            if (Find.TickManager.TicksGame - spawnTick > 1500)
            {
                Destroy();
                return;
            }

            if (jobs.curJob.def.defName != "XFMParmor_Job_ReturnToCarrier")
            {
                var job = JobMaker.MakeJob(JobDefOf.XFMParmor_Job_ReturnToCarrier, origin.ThingFixed, -1);
                jobs.TryTakeOrderedJob(job, JobTag.Misc);
            }
        }

        ShieldClass.Tick();
        if (ShieldClass.Shield <= 0f)
        {
            Destroy();
        }
    }

    protected virtual void ShowImpectFleck(Vector3 drawPos, float amout)
    {
        RimWorld.SoundDefOf.EnergyShield_AbsorbDamage.PlayOneShot(new TargetInfo(drawPos.ToIntVec3(), Map));
        MakeFlecks(drawPos, amout);
    }

    private void MakeFlecks(Vector3 drawPos, float amout)
    {
        var num = Math.Max(Rand.Range(amout / 40f, 1f + (amout / 30f)), 1.5f);
        FleckMaker.Static(drawPos, Map, RimWorld.FleckDefOf.ExplosionFlash, num);
        var num2 = (int)(num / 0.5f) + Rand.Range(1, 3);
        for (var i = 0; i < num2; i++)
        {
            Map.flecks.CreateFleck(new FleckCreationData
            {
                def = FleckDefOf.XFMParmor_ShieldChip,
                scale = num * 0.2f,
                spawnPosition = drawPos,
                instanceColor = new Color(1f, 1f, Rand.Range(0.8f, 1f), Rand.Range(0.7f, 0.9f)),
                rotation = Rand.Range(0f, 360f),
                velocitySpeed = num + Rand.Range(0.2f, 0.5f),
                velocityAngle = Rand.Range(0f, 360f)
            });
        }
    }

    protected override void DrawAt(Vector3 drawLoc, bool flip = false)
    {
        base.DrawAt(drawLoc, flip);
        if (isAttactMode)
        {
            return;
        }

        drawLoc.y = AltitudeLayer.Blueprint.AltitudeFor();
        var s = new Vector3(12.349999f, 1f, 12.349999f);
        var matrix = default(Matrix4x4);
        matrix.SetTRS(drawLoc, Quaternion.AngleAxis(0f, Vector3.up), s);
        Graphics.DrawMesh(MeshPool.plane10, matrix, MaterialOf.DroneShield, 0);
    }

    public override IEnumerable<Gizmo> GetGizmos()
    {
        foreach (var gizmo in ShieldClass.GetGizmos("Shield", GetBattery()))
        {
            yield return gizmo;
        }

        if (isAttactMode)
        {
            yield return new Command_Target
            {
                defaultLabel = "XFMParmor_Drone_GizmoAttactA".Translate(),
                defaultDesc = "XFMParmor_Drone_GizmoAttactB".Translate(),
                icon = Texture2DOf.Attact,
                targetingParams = new TargetingParameters
                {
                    canTargetLocations = true,
                    canTargetBuildings = true,
                    canTargetItems = false,
                    validator = tinfo =>
                        tinfo.Map == Map && tinfo.IsValid && this.CanReach(tinfo.Cell, PathEndMode.Touch, Danger.Deadly)
                },
                action = delegate(LocalTargetInfo tinfo)
                {
                    if (tinfo.HasThing)
                    {
                        jobs.StopAll();
                        SetMeleeTarget(tinfo);
                    }
                    else
                    {
                        var job = JobMaker.MakeJob(RimWorld.JobDefOf.Goto, tinfo.Cell, -1, true);
                        jobs.TryTakeOrderedJob(job, JobTag.Misc);
                    }
                }
            };
        }
        else
        {
            yield return new Command_Target
            {
                defaultLabel = "XFMParmor_Drone_GizmoDefendA".Translate(),
                defaultDesc = "XFMParmor_Drone_GizmoDefendB".Translate(),
                icon = Texture2DOf.Attact,
                targetingParams = new TargetingParameters
                {
                    canTargetLocations = true,
                    canTargetBuildings = false,
                    canTargetItems = false,
                    validator = tinfo =>
                        tinfo.Map == Map && tinfo.IsValid && this.CanReach(tinfo.Cell, PathEndMode.Touch, Danger.Deadly)
                },
                action = delegate(LocalTargetInfo tinfo)
                {
                    forceTarget = tinfo;
                    jobs.StopAll();
                }
            };
        }
    }

    private void SetMeleeTarget(LocalTargetInfo tinfo)
    {
        mindState.enemyTarget = tinfo.Thing;
        mindState.lastEngageTargetTick = Find.TickManager.TicksGame;
        var job = JobMaker.MakeJob(RimWorld.JobDefOf.AttackMelee, tinfo);
        job.expiryInterval = new IntRange(360, 480).RandomInRange;
        job.checkOverrideOnExpire = true;
        job.expireRequiresEnemiesNearby = true;
        jobs.TryTakeOrderedJob(job, JobTag.Misc);
    }

    private float GetBattery()
    {
        return 1f - Math.Min((Find.TickManager.TicksGame - spawnTick) / 900f, 1f);
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_References.Look(ref origin, "XForigin");
        Scribe_Values.Look(ref spawnTick, "fillPercent");
        Scribe_Values.Look(ref isAttactMode, "isAttactMode");
        Scribe_Values.Look(ref searthNewTarget, "searthNewTarget", true);
        Scribe_Deep.Look(ref shieldClass, "shieldClass", new object());
        Scribe_TargetInfo.Look(ref forceTarget, "followTarget");
    }
}