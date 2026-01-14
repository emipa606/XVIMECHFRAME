using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace MParmorLibrary;

public static class ToolsLibrary
{
    public static readonly IEnumerable<IntVec3> NineCellsField = new List<IntVec3>
    {
        new(0, 0, 0),
        new(0, 0, 1),
        new(0, 0, -1),
        new(1, 0, 0),
        new(-1, 0, 0),
        new(1, 0, 1),
        new(1, 0, -1),
        new(-1, 0, 1),
        new(-1, 0, -1)
    };

    public static bool IsUnfriendly(DamageInfo dinfo, Thing victim)
    {
        return IsUnfriendly(dinfo.Instigator, victim);
    }

    public static bool IsUnfriendly(Thing a, Thing b)
    {
        return a == null || b == null || a.Faction == null || b.Faction == null || a.Faction.HostileTo(b.Faction);
    }

    public static bool IsUnfriendly(Projectile a, Thing b)
    {
        return a.Launcher == null || b == null || a.Launcher.Faction == null || b.Faction == null ||
               a.Launcher.Faction.HostileTo(b.Faction);
    }

    public static bool IsUnfriendly(Faction a, Faction b)
    {
        return a == null || b == null || a.HostileTo(b);
    }

    public static void ReLaunchProjectile(Projectile original, Thing caster, LocalTargetInfo target)
    {
        LaunchProjectile(original.def,
            original.EquipmentDef != null ? ThingMaker.MakeThing(original.EquipmentDef) : null, caster, target,
            original.DrawPos);
    }

    public static void LaunchProjectile(ThingDef projectileDef, Thing equipment, Thing caster, LocalTargetInfo target,
        Vector3? castPos = null, Map castMap = null)
    {
        var shootLine = new ShootLine((castPos ?? caster.DrawPos).ToIntVec3(), target.Cell);
        var projectile = (Projectile)GenSpawn.Spawn(projectileDef, shootLine.Source, castMap ?? caster.Map);
        projectile.Launch(caster, castPos ?? caster.DrawPos, target, target, ProjectileHitFlags.All, false, equipment);
    }

    public static Rot4 RotFromAngle(float angle)
    {
        if (angle > 360f)
        {
            angle %= 360f;
        }

        if (angle < 45f)
        {
            return Rot4.North;
        }

        if (angle < 135f)
        {
            return Rot4.East;
        }

        if (angle < 225f)
        {
            return Rot4.South;
        }

        return angle < 315f ? Rot4.West : Rot4.North;
    }

    public static Rot4 RotFromPosition(Vector3 a, Vector3 b)
    {
        return RotFromAngle((b - a).AngleFlat());
    }

    public static bool IsChurk(List<ThingCategoryDef> thingCategory)
    {
        foreach (var item in thingCategory)
        {
            if (IsChurk(item))
            {
                return true;
            }
        }

        return false;
    }

    public static bool IsChurk(ThingCategoryDef thingCategory)
    {
        return thingCategory == ThingCategoryDefOf.Chunks ||
               ThingCategoryDefOf.Chunks.childCategories.Contains(thingCategory);
    }

    public static float GetDistance(Vector3 a, Vector3 b, bool sqrt = false)
    {
        var num = ((a.x - b.x) * (a.x - b.x)) + ((a.z - b.z) * (a.z - b.z));
        if (sqrt)
        {
            num = (float)Math.Sqrt(num);
        }

        return num;
    }

    public static float GetDistance(IntVec3 a, IntVec3 b, bool sqrt = false)
    {
        var a2 = a.ToVector3Shifted();
        var b2 = b.ToVector3Shifted();
        return GetDistance(a2, b2, sqrt);
    }

    public static bool CollisionDetermination(Vector3 a, Vector3 b, float range)
    {
        return GetDistance(a, b) <= range * range;
    }

    public static bool CollisionDetermination(Vector3 a, Vector3 b, float range, float rangeLimit)
    {
        return CollisionDetermination(a, b, range) && !CollisionDetermination(a, b, rangeLimit);
    }

    public static bool AngleAlgorithm(float angleA, float angleB, float limit)
    {
        return Math.Abs(angleA - angleB) <= limit * 0.5 || Math.Abs(angleA - 360f - angleB) <= limit * 0.5 ||
               Math.Abs(angleA + 360f - angleB) <= limit * 0.5;
    }

    public static bool IsInSector(Vector3 vertex, Vector3 postion, float radius, float targetAngle, float angle)
    {
        return CollisionDetermination(vertex, postion, radius) &&
               AngleAlgorithm((postion - vertex).AngleFlat(), targetAngle, angle);
    }

    public static bool IsInSector(Vector3 vertex, Vector3 postion, float targetAngle, float angle)
    {
        return AngleAlgorithm((postion - vertex).AngleFlat(), targetAngle, angle);
    }

    public static Color RemoveColorAlpha(Color color)
    {
        return new Color(color.r, color.g, color.b, 1f);
    }

    public static void FillableBarByRot4(Rect rect, float fillPercent, Rot4 rotation, Texture2D fillTex,
        Texture2D bgTex = null, bool doBorder = false, float borderSize = 3f, Texture2D borderTex = null)
    {
        if (doBorder)
        {
            GUI.DrawTexture(rect, borderTex ?? BaseContent.BlackTex);
            rect = rect.ContractedBy(borderSize);
        }

        if (bgTex != null)
        {
            GUI.DrawTexture(rect, bgTex);
        }

        if (rotation == Rot4.East || rotation == Rot4.West)
        {
            if (rotation == Rot4.West)
            {
                rect.x += rect.width;
                fillPercent *= -1f;
            }

            rect.width *= fillPercent;
        }
        else
        {
            if (rotation == Rot4.North)
            {
                rect.y += rect.height;
                fillPercent *= -1f;
            }

            rect.height *= fillPercent;
        }

        GUI.DrawTexture(rect, fillTex);
    }

    public static void DrawGraphicWithLayer(GraphicData graphicData, Vector3 drawPos, float extraRotation = 0f,
        Rot4? rotation = null, AltitudeLayer? layer = null, Color? color = null, Color? color2 = null,
        Shader newShader = null, ThingDef thingDef = null, Thing thing = null)
    {
        if (layer.HasValue)
        {
            drawPos.y = layer.Value.AltitudeFor();
        }

        var graphic = graphicData.Graphic;
        if (graphicData.Linked)
        {
            var graphic_Linked = GraphicUtility.WrapLinked(graphic, graphicData.linkType);
            graphic_Linked =
                graphic_Linked.GetColoredVersion(
                    newShader ?? (graphicData.shaderType == null
                        ? ShaderTypeDefOf.Cutout.Shader
                        : graphicData.shaderType.Shader), color ?? graphicData.color,
                    color2 ?? graphicData.colorTwo) as Graphic_Linked;
            if (thing == null)
            {
                return;
            }

            var mesh = graphic_Linked?.MeshAt(thing.Rotation);
            var material = graphic_Linked?.MatSingleFor(thing);
            Graphics.DrawMesh(mesh, drawPos, Quaternion.identity, material, 0);
        }
        else
        {
            graphic = graphic.GetColoredVersion(
                newShader ?? (graphicData.shaderType == null
                    ? ShaderTypeDefOf.Cutout.Shader
                    : graphicData.shaderType.Shader), color ?? graphicData.color, color2 ?? graphicData.colorTwo);
            graphic.DrawWorker(drawPos, rotation ?? Rot4.North, thingDef, thing,
                extraRotation);
        }
    }

    public static void SpawnThingDefCount(ThingDefCountClass thing, IntVec3 position, Map map)
    {
        var num = thing.count;
        var check = true;
        while (check)
        {
            var thing2 = ThingMaker.MakeThing(thing.thingDef);
            if (num > thing.thingDef.stackLimit)
            {
                thing2.stackCount = thing.thingDef.stackLimit;
                num -= thing.thingDef.stackLimit;
            }
            else
            {
                thing2.stackCount = num;
                check = false;
            }

            GenSpawn.Spawn(thing2, position, map, WipeMode.VanishOrMoveAside);
        }
    }

    public static void SpawnThingDefCount(IEnumerable<ThingDefCountClass> things, IntVec3 position, Map map)
    {
        foreach (var thing in things)
        {
            SpawnThingDefCount(thing, position, map);
        }
    }

    public static float HealSingle(float amount, Pawn pawn)
    {
        var resultHediffs = new List<Hediff_Injury>();
        pawn.health.hediffSet.GetHediffs(ref resultHediffs);
        var hediff_Injury = resultHediffs.Where(x => x.CanHealNaturally() && x.Severity > 0f).RandomElement();
        if (hediff_Injury == null)
        {
            return amount;
        }

        var result = hediff_Injury.Severity > amount ? amount : hediff_Injury.Severity;
        hediff_Injury.Heal(amount);
        if (hediff_Injury.Severity <= 0f)
        {
            hediff_Injury.PostRemoved();
        }

        return result;
    }

    public static float HealMassively(float amountFloat, Pawn pawn)
    {
        var num = 0f;
        var num2 = amountFloat;
        var num3 = 0;
        while (pawn.health.hediffSet.HasNaturallyHealingInjury() && num2 > 0f && num3 < 20)
        {
            var num4 = HealSingle(num2, pawn);
            num2 -= num4;
            num += num4;
            num3++;
        }

        return num;
    }

    public static Vector3 V2ToV3(this Vector2 vector2, float y = 0f)
    {
        return new Vector3(vector2.x, y, vector2.y);
    }

    public static Vector2 V3ToV2(this Vector3 vector3)
    {
        return new Vector2(vector3.x, vector3.z);
    }

    public static void SendStandardLetter(TaggedString baseLetterLabel, TaggedString baseLetterText,
        LetterDef baseLetterDef, IncidentParms parms, LookTargets lookTargets, params NamedArgument[] textArgs)
    {
        if (!parms.sendLetter)
        {
            return;
        }

        if (baseLetterLabel.NullOrEmpty() || baseLetterText.NullOrEmpty())
        {
            Log.Error("Sending standard incident letter with no label or text.");
        }

        var taggedString = baseLetterText.Formatted(textArgs);
        TaggedString text;
        if (parms.customLetterText.NullOrEmpty())
        {
            text = taggedString;
        }
        else
        {
            var list = new List<NamedArgument>();
            if (textArgs != null)
            {
                list.AddRange(textArgs);
            }

            list.Add(taggedString.Named("BASETEXT"));
            text = parms.customLetterText.Formatted(list.ToArray());
        }

        var taggedString2 = baseLetterLabel.Formatted(textArgs);
        TaggedString label;
        if (parms.customLetterLabel.NullOrEmpty())
        {
            label = taggedString2;
        }
        else
        {
            var list2 = new List<NamedArgument>();
            if (textArgs != null)
            {
                list2.AddRange(textArgs);
            }

            list2.Add(taggedString2.Named("BASELABEL"));
            label = parms.customLetterLabel.Formatted(list2.ToArray());
        }

        var choiceLetter = LetterMaker.MakeLetter(label, text, parms.customLetterDef ?? baseLetterDef, lookTargets,
            parms.faction, parms.quest, parms.letterHyperlinkThingDefs);
        var list3 = new List<HediffDef>();
        if (!parms.letterHyperlinkHediffDefs.NullOrEmpty())
        {
            list3.AddRange(parms.letterHyperlinkHediffDefs);
        }

        choiceLetter.hyperlinkHediffDefs = list3;
        Find.LetterStack.ReceiveLetter(choiceLetter);
    }

    public static List<IntVec3> GetCellsInRect(IntVec3 pointA, IntVec3 pointB, int edge = 0)
    {
        var intVec = new IntVec3(Math.Min(pointA.x, pointB.x) - edge, 0, Math.Min(pointA.z, pointB.z) - edge);
        var intVec2 = new IntVec3(Math.Max(pointA.x, pointB.x) + edge, 0, Math.Max(pointA.z, pointB.z) + edge);
        var num = intVec2.x - intVec.x + 1;
        var num2 = intVec2.z - intVec.z + 1;
        var list = new List<IntVec3>();
        for (var i = 0; i < num; i++)
        {
            for (var j = 0; j < num2; j++)
            {
                list.Add(intVec + new IntVec3(i, 0, j));
            }
        }

        return list;
    }

    public static float GetDistanceBetweenLineAndPoint(Vector2 endpointA, Vector2 endpointB, Vector2 outsidePoint,
        bool sqrt = false)
    {
        if (endpointA == endpointB)
        {
            return GetDistance(endpointA, outsidePoint, sqrt);
        }

        var num =
            (((endpointB.x - endpointA.x) * (outsidePoint.x - endpointA.x)) +
             ((endpointB.y - endpointA.y) * (outsidePoint.y - endpointA.y))) /
            (((endpointB.x - endpointA.x) * (endpointB.x - endpointA.x)) +
             ((endpointB.y - endpointA.y) * (endpointB.y - endpointA.y)));
        var vector = default(Vector2);
        if (num < 0f)
        {
            vector.x = endpointA.x;
            vector.y = endpointA.y;
        }

        if (num is >= 0f and <= 1f)
        {
            vector.x = endpointA.x + (num * (endpointB.x - endpointA.x));
            vector.y = endpointA.y + (num * (endpointB.y - endpointA.y));
        }

        if (num > 1f)
        {
            vector.x = endpointB.x;
            vector.y = endpointB.y;
        }

        var num2 = ((outsidePoint.x - vector.x) * (outsidePoint.x - vector.x)) +
                   ((outsidePoint.y - vector.y) * (outsidePoint.y - vector.y));
        if (sqrt)
        {
            num2 = (float)Math.Sqrt(num2);
        }

        return num2;
    }

    public static float GetDistanceBetweenLineAndPoint(Vector3 endpointA, Vector3 endpointB, Vector3 outsidePoint,
        bool sqrt = false)
    {
        return GetDistanceBetweenLineAndPoint(new Vector2(endpointA.x, endpointA.z),
            new Vector2(endpointB.x, endpointB.z), new Vector2(outsidePoint.x, outsidePoint.z), sqrt);
    }

    public static BodyPartRecord GetOutsideBodyPart(BodyPartRecord bodyPart)
    {
        if (bodyPart == null)
        {
            return null;
        }

        while (bodyPart.depth == BodyPartDepth.Inside && bodyPart.parent != null &&
               !(bodyPart.parent.coverageAbs <= 0f))
        {
            bodyPart = bodyPart.parent;
        }

        return bodyPart;
    }

    public static List<BodyPartRecord> GetOutsideBodyParts(BodyPartRecord bodyPart)
    {
        var list = new List<BodyPartRecord>();
        if (bodyPart == null)
        {
            return list;
        }

        list.Add(bodyPart);
        while (bodyPart.depth == BodyPartDepth.Inside && bodyPart.parent != null &&
               !(bodyPart.parent.coverageAbs <= 0f))
        {
            bodyPart = bodyPart.parent;
            list.Add(bodyPart);
        }

        return list;
    }

    public static List<IntVec3> NineCells()
    {
        return
        [
            new IntVec3(0, 0, 0),
            new IntVec3(0, 0, 1),
            new IntVec3(0, 0, -1),
            new IntVec3(1, 0, 0),
            new IntVec3(-1, 0, 0),
            new IntVec3(1, 0, 1),
            new IntVec3(1, 0, -1),
            new IntVec3(-1, 0, 1),
            new IntVec3(-1, 0, -1)
        ];
    }

    public static List<IntVec3> EightCells()
    {
        return
        [
            new IntVec3(0, 0, 1),
            new IntVec3(0, 0, -1),
            new IntVec3(1, 0, 0),
            new IntVec3(-1, 0, 0),
            new IntVec3(1, 0, 1),
            new IntVec3(1, 0, -1),
            new IntVec3(-1, 0, 1),
            new IntVec3(-1, 0, -1)
        ];
    }

    extension(Pawn pawn)
    {
        public bool TryFindThingByDefFromInventory(string defName, out Thing foundThing)
        {
            foundThing = null;
            using var enumerator = (from Thing thing in pawn.inventory.GetDirectlyHeldThings()
                where thing.def.defName == defName
                select thing).GetEnumerator();
            if (!enumerator.MoveNext())
            {
                return false;
            }

            var current = enumerator.Current;
            foundThing = current;
            return true;
        }

        public bool TryFindThingByWeaponTagFromInventory(string tag, out Thing foundThing)
        {
            foundThing = null;
            using var enumerator = (from Thing thing in pawn.inventory.GetDirectlyHeldThings()
                where thing.def.weaponTags != null && thing.def.weaponTags.Contains(tag)
                select thing).GetEnumerator();
            if (!enumerator.MoveNext())
            {
                return false;
            }

            var current = enumerator.Current;
            foundThing = current;
            return true;
        }
    }
}