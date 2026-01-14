using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace MParmorLibrary;

public class Building_IceSpike : Building
{
    private const int wickTick = 360;

    private bool actived;
    public Thing owner;

    private int wickTicks;

    private float WickPercentage => wickTicks < 0 ? 0f : wickTicks / 360f;

    protected override void DrawAt(Vector3 drawLoc, bool flip = false)
    {
        Graphic.GetColoredVersion(ShaderTypeDefOf.Cutout.Shader,
                Color.Lerp(Color.white, new Color(0f, 0.75f, 1f), WickPercentage), Color.white)
            .Draw(drawLoc, flip ? Rotation.Opposite : Rotation, this);
    }

    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        base.SpawnSetup(map, respawningAfterLoad);
        wickTicks += Rand.Range(-300, -180);
    }

    private bool CheckTarget(Thing thing)
    {
        if (thing is Pawn b)
        {
            return ToolsLibrary_MParmorOnly.IsUnfriendly(owner, b);
        }

        return false;
    }

    public override void PreApplyDamage(ref DamageInfo dinfo, out bool absorbed)
    {
        absorbed = false;
        if (dinfo.Def != DamageDefOf.MParmor_Bomb_IceSpike)
        {
            return;
        }

        absorbed = true;
        Active();
    }

    protected override void Tick()
    {
        if (WickPercentage >= 1f)
        {
            var thing = owner;
            var position = Position;
            var map = Map;
            Destroy();
            GenExplosion.DoExplosion(position, map, 2.9f, DamageDefOf.MParmor_Bomb_IceSpike, thing, 24, 0.9f, null,
                null, null, null, null, 0f, 1, null, null, 0);
            return;
        }

        wickTicks += !actived ? 1 : 12;
        if (actived)
        {
            return;
        }

        foreach (var thing2 in Position.GetThingList(Map))
        {
            if (!CheckTarget(thing2))
            {
                continue;
            }

            Active();
            break;
        }
    }

    private void Active()
    {
        var list = new List<Thing>();
        foreach (var thing2 in Position.GetThingList(Map))
        {
            if (CheckTarget(thing2))
            {
                list.Add(thing2);
            }
        }

        while (list.Count > 0)
        {
            var thing = list[^1];
            var dinfo = new DamageInfo(RimWorld.DamageDefOf.Stab, 24f, 20f, -1f, owner);
            dinfo.SetBodyRegion(BodyPartHeight.Bottom);
            thing.TakeDamage(dinfo);
            list.RemoveAt(list.Count - 1);
        }

        if (wickTicks < 0)
        {
            wickTicks = 0;
        }

        actived = true;
    }
}