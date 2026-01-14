using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace MParmorLibrary;

public class Wall_DescendingWall : Building
{
    public static List<Wall_DescendingWall> instances = [];
    public bool isSelectored;

    public bool IsDescending => def == ThingDefOf.XFMParmor_DescendingWall_Descending;

    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        base.SpawnSetup(map, respawningAfterLoad);
        instances.Add(this);
    }

    public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
    {
        base.DeSpawn(mode);
        instances.Remove(this);
    }

    public override IEnumerable<Gizmo> GetGizmos()
    {
        foreach (var gizmo in base.GetGizmos())
        {
            yield return gizmo;
        }

        yield return Change();
        yield return Select();
    }

    protected virtual Command_Action Change()
    {
        var command_Action = new Command_Action
        {
            defaultLabel = IsDescending
                ? "XFMParmor_Wall_DescendingWallA".Translate()
                : "XFMParmor_Wall_DescendingWallB".Translate(),
            defaultDesc = "XFMParmor_Wall_DescendingWallAd".Translate(),
            action = delegate
            {
                var position = Position;
                var map = Map;
                var wall_DescendingWall =
                    ThingMaker.MakeThing(
                        IsDescending
                            ? ThingDefOf.XFMParmor_DescendingWall
                            : ThingDefOf.XFMParmor_DescendingWall_Descending, Stuff) as Wall_DescendingWall;
                wall_DescendingWall?.SetFaction(Faction);
                wall_DescendingWall?.HitPoints = HitPoints;
                FleckMaker.ThrowDustPuff(
                    position.ToVector3Shifted() + new Vector3(Rand.Range(0.15f, -0.15f), 0f, Rand.Range(0.15f, -0.15f)),
                    map, 1.1f);
                FleckMaker.ThrowDustPuff(
                    position.ToVector3Shifted() + new Vector3(Rand.Range(0.15f, -0.15f), 0f, Rand.Range(0.15f, -0.15f)),
                    map, 1.1f);
                Destroy();
                GenSpawn.Spawn(wall_DescendingWall, position, map);
                Find.Selector.Select(wall_DescendingWall, false);
            }
        };
        if (!((CompPowerTrader)PowerComp).PowerOn)
        {
            command_Action.Disable("NoPower".Translate());
            return command_Action;
        }

        if (Find.TickManager.slower.ForcedNormalSpeed)
        {
            command_Action.Disable("XFMParmor_Wall_DescendingWallC".Translate());
        }

        return command_Action;
    }

    protected virtual Command_Action Select()
    {
        return new Command_Action
        {
            defaultLabel = "XFMParmor_Wall_DescendingWallD".Translate(),
            defaultDesc = "XFMParmor_Wall_DescendingWallDd".Translate(),
            action = delegate
            {
                foreach (var instance in instances)
                {
                    instance.isSelectored = false;
                }

                RimWorld.SoundDefOf.ThingSelected.PlayOneShotOnCamera();
                Connect(this);
            }
        };
    }

    private void Connect(Wall_DescendingWall target)
    {
        if (target is not { Spawned: true } || target.def != def || target.isSelectored)
        {
            return;
        }

        target.isSelectored = true;
        Find.Selector.Select(target, false);
        Connect((target.Position + new IntVec3(1, 0, 0)).GetFirstThing<Wall_DescendingWall>(Map));
        Connect((target.Position + new IntVec3(-1, 0, 0)).GetFirstThing<Wall_DescendingWall>(Map));
        Connect((target.Position + new IntVec3(0, 0, 1)).GetFirstThing<Wall_DescendingWall>(Map));
        Connect((target.Position + new IntVec3(0, 0, -1)).GetFirstThing<Wall_DescendingWall>(Map));
    }
}