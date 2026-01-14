using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace MParmorLibrary;

public class Bullet_BloodIce : Bullet
{
    private readonly List<Thing> things = [];
    public Pawn bloodSource;

    public override bool AnimalsFleeImpact => false;

    protected override void Tick()
    {
        if (Spawned && !landed)
        {
            things.AddRange(Position.GetThingList(Map));
            foreach (var thing in things)
            {
                if (thing is not Pawn pawn || !CanHit(pawn))
                {
                    continue;
                }

                Impact(pawn);
                return;
            }
        }

        things.Clear();
        base.Tick();
    }

    protected override void Impact(Thing hitThing, bool blockedByShield = false)
    {
        DropBloodFilth();
        var map = Map;
        var position = Position;
        GenClamor.DoClamor(this, 12f, ClamorDefOf.Impact);
        Destroy();
        NotifyImpact(hitThing, map, position);
        if (hitThing != null)
        {
            var dinfo = new DamageInfo(def.projectile.damageDef, DamageAmount, base.ArmorPenetration,
                ExactRotation.eulerAngles.y, launcher, null, equipmentDef, DamageInfo.SourceCategory.ThingOrUnknown,
                intendedTarget.Thing);
            hitThing.TakeDamage(dinfo);
            if (hitThing is Pawn { stances: not null } pawn && pawn.BodySize <= def.projectile.stoppingPower + 0.001f)
            {
                pawn.stances.stagger.StaggerFor(95);
            }

            if (def.projectile.extraDamages == null)
            {
                return;
            }

            {
                foreach (var extraDamage in def.projectile.extraDamages)
                {
                    if (!Rand.Chance(extraDamage.chance))
                    {
                        continue;
                    }

                    var dinfo2 = new DamageInfo(extraDamage.def, extraDamage.amount,
                        extraDamage.AdjustedArmorPenetration(), ExactRotation.eulerAngles.y, launcher, null,
                        equipmentDef, DamageInfo.SourceCategory.ThingOrUnknown, intendedTarget.Thing);
                    hitThing.TakeDamage(dinfo2);
                }

                return;
            }
        }

        RimWorld.SoundDefOf.BulletImpact_Ground.PlayOneShot(new TargetInfo(Position, map));
        if (Position.GetTerrain(map).takeSplashes)
        {
            FleckMaker.WaterSplash(ExactPosition, map, Mathf.Sqrt(DamageAmount) * 1f, 4f);
        }
        else
        {
            FleckMaker.Static(ExactPosition, map, RimWorld.FleckDefOf.ShotHit_Dirt);
        }
    }

    private void NotifyImpact(Thing hitThing, Map map, IntVec3 position)
    {
        var impactData = new BulletImpactData
        {
            bullet = this,
            hitThing = hitThing,
            impactPosition = position
        };
        hitThing?.Notify_BulletImpactNearby(impactData);
        var num = 9;
        for (var i = 0; i < num; i++)
        {
            var c = position + GenRadial.RadialPattern[i];
            if (!c.InBounds(map))
            {
                continue;
            }

            var thingList = c.GetThingList(map);
            foreach (var thing in thingList)
            {
                if (thing != hitThing)
                {
                    thing.Notify_BulletImpactNearby(impactData);
                }
            }
        }
    }

    private void DropBloodFilth()
    {
        if (bloodSource != null && (bloodSource.Spawned || bloodSource.ParentHolder is Pawn_CarryTracker) &&
            bloodSource.SpawnedOrAnyParentSpawned && bloodSource.RaceProps.BloodDef != null)
        {
            FilthMaker.TryMakeFilth(Position, Map, bloodSource.RaceProps.BloodDef, bloodSource.LabelIndefinite());
        }
    }
}