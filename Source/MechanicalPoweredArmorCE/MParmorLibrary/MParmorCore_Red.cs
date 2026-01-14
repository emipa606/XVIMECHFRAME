using System;
using System.Collections.Generic;
using System.Linq;
using CombatExtended;
using MParmorLibrary.SingleObject;
using MParmorLibrary.SkillSystem;
using RimWorld;
using UnityEngine;
using Verse;

namespace MParmorLibrary;

public class MParmorCore_Red : MParmorCore
{
    private const int parryTick = 12;

    public int activeOutbreakUntilTick = -1;

    public int activeTime = -1;
    public int attackSpeed;

    public int attackSpeedMax = 90;

    public int attactSpeedLevel;

    public float range = 1.5f;

    public override bool CanChargeSkills => base.CanChargeSkills && Wearer.Spawned;

    public bool IsActiveMode => activeOutbreakUntilTick >= Find.TickManager.TicksGame;

    public float ActivePercent => (activeOutbreakUntilTick - Find.TickManager.TicksGame) / (float)activeTime;

    public override void ReloadAmmoForPrimary()
    {
    }

    public override IEnumerable<Gizmo> GetExtraSkillGizmos()
    {
        var color = Color.white;
        color.a = 0.2f;
        yield return new Gizmo_BlockWithFillBar
        {
            Order = -900f,
            defaultLabel = "XFMParmorCore_Red_AttackSpeedA".Translate(),
            defaultDesc = "XFMParmorCore_Red_AttackSpeedB".Translate(),
            topRightLabel = "{0}/4".Formatted(attactSpeedLevel.ToString()),
            fillColor = color,
            fillPercent = attackSpeed / (float)attackSpeedMax,
            icon = Texture2DOf.RedAttactSpeed
        };
    }

    protected override bool PreTakeDamage(DamageInfo dinfo)
    {
        var skillObject = GetComp<CompSkills>().FindSkill("VigorouslyRebound");
        if (skillObject == null)
        {
            return false;
        }

        if (skillObject.CanUsed(out _))
        {
            return false;
        }

        if (!dinfo.Def.harmsHealth)
        {
            return false;
        }

        if (dinfo.Instigator == Wearer)
        {
            return false;
        }

        if (dinfo.Instigator != null && !(ToolsLibrary.GetDistance(dinfo.Instigator.Position, Wearer.Position) > 2.1))
        {
            return false;
        }

        var num = dinfo.Amount / 2f;
        if (num > 60f)
        {
            num = 60f;
        }

        skillObject.ChargeEnergy((int)num);

        return false;
    }

    public bool TryReboundProjectiles(ProjectileCE projectile)
    {
        if (!IsActiveMode)
        {
            return false;
        }

        if (ToolsLibrary_CEonly.IsUnfriendly(projectile, Wearer) && projectile.launcher != null &&
            projectile.launcher is not Pawn { Downed: not false })
        {
            ToolsLibrary_CEonly.LaunchProjectile(projectile.def, ThingMaker.MakeThing(projectile.equipmentDef),
                Wearer, projectile.launcher);
        }

        projectile.Destroy();
        return true;
    }

    protected override void TickMParmor(out bool returnNow)
    {
        returnNow = false;
        if (attackSpeed > 0 && Wearer.Spawned && Find.TickManager.TicksGame % 2 == 0)
        {
            attackSpeed--;
            if (attackSpeed == 0)
            {
                attactSpeedLevel--;
                if (attactSpeedLevel > 0)
                {
                    attackSpeed = AttackSpeedMax(attactSpeedLevel);
                }
            }
        }

        if (!IsActiveMode)
        {
            return;
        }

        var list = new List<Projectile>();
        foreach (var item in ProjectilesStore.GetInstance().projectiles
                     .Where(projectile => projectile.Spawned && !projectile.Destroyed))
        {
            if (item.Map == Wearer.Map && ToolsLibrary_MParmorOnly.IsUnfriendly(item, Wearer) &&
                item.Launcher != Wearer && ToolsLibrary.CollisionDetermination(item.ExactPosition, Wearer.TrueCenter(),
                    Math.Max(range, range * item.def.projectile.SpeedTilesPerTick)))
            {
                list.Add(item);
            }
        }

        while (list.Count > 0)
        {
            if (Wearer.pather.Moving)
            {
                Wearer.stances.SetStance(
                    new Stance_Parry(1, new LocalTargetInfo(new ThingWithDrawPos(list[0].DrawPos))));
            }
            else
            {
                Wearer.stances.SetStance(new Stance_Parry(12,
                    new LocalTargetInfo(new ThingWithDrawPos(list[0].DrawPos))));
            }

            if (list[0].Launcher != null && list[0].Launcher is not Pawn { Downed: not false } && list[0].Launcher is
                {
                    Spawned: true
                })
            {
                var localTargetInfo = new LocalTargetInfo(list[0].Launcher);
                ToolsLibrary.ReLaunchProjectile(list[0], Wearer,
                    DirectHitTarget(localTargetInfo)
                        ? localTargetInfo
                        : new LocalTargetInfo(localTargetInfo.Cell + ToolsLibrary.EightCells().RandomElement()));
            }

            list[0].Destroy();
            list.Remove(list[0]);
        }
    }

    private int AttackSpeedMax(int level)
    {
        attackSpeedMax = (AttactSpeedTime(level) * 2) + 30;
        return attackSpeedMax;
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref activeOutbreakUntilTick, "activeOutbreakUntilTick", -1);
        Scribe_Values.Look(ref activeTime, "activeTime", -1);
        Scribe_Values.Look(ref range, "range", 1.5f);
        Scribe_Values.Look(ref attackSpeed, "attackSpeed");
        Scribe_Values.Look(ref attackSpeedMax, "attackSpeedMax", 90);
        Scribe_Values.Look(ref attactSpeedLevel, "attactSpeedLevel");
    }

    private bool DirectHitTarget(LocalTargetInfo localTarget)
    {
        var distance = ToolsLibrary.GetDistance(Wearer.DrawPos, localTarget.CenterVector3, true);
        distance -= 10f;
        var num = 100 - (int)(distance * 100f / 20f);
        return Rand.Range(1, 101) <= num;
    }

    public static int AttactSpeedTime(int level)
    {
        return 90 - (level * 15);
    }

    public void AttactSpeedLevelUp(int lastLevel)
    {
        if (lastLevel < 4)
        {
            attactSpeedLevel++;
        }

        attackSpeed = AttackSpeedMax(lastLevel);
    }
}