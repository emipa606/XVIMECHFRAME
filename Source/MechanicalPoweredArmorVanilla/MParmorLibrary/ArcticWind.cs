using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace MParmorLibrary;

public class ArcticWind : Thing_PeriodicAction
{
    public const float Range = 7.9f;
    public Pawn source;

    public override int Cycle => 90;

    public override int Age => 360;

    private Graphic GraphicA
    {
        get
        {
            field ??= new GraphicData
            {
                graphicClass = typeof(Graphic_Single),
                texPath = "XFMParmor/MParmor/Aqua/ArcticWindA",
                shaderType = ShaderTypeDefOf.Transparent,
                drawSize = new Vector2(16.8f, 16.8f)
            }.Graphic;

            return field;
        }
    }

    private Graphic GraphicB
    {
        get
        {
            field ??= new GraphicData
            {
                graphicClass = typeof(Graphic_Single),
                texPath = "XFMParmor/MParmor/Aqua/ArcticWindB",
                shaderType = ShaderTypeDefOf.Transparent,
                drawSize = new Vector2(16.8f, 16.8f)
            }.Graphic;

            return field;
        }
    }

    public override void SpawnAction()
    {
        TickAction();
    }

    protected override void DrawAt(Vector3 drawLoc, bool flip = false)
    {
        drawLoc.y = AltitudeLayer.Blueprint.AltitudeFor();
        float num = Find.TickManager.TicksGame % 360;
        var num2 = Find.TickManager.TicksGame - TickTime;
        var num3 = num2 > 330 ? 1f - ((num2 - 330) / 30f) : 1f;
        GraphicA.GetColoredVersion(ShaderTypeDefOf.Transparent.Shader, new Color(1f, 1f, 1f, 0.3f * num3), Color.white)
            .DrawWorker(drawLoc, Rot4.North, def, this, num * 0.5f);
        GraphicB.GetColoredVersion(ShaderTypeDefOf.Transparent.Shader, new Color(1f, 1f, 1f, 0.2f * num3), Color.white)
            .Draw(drawLoc, Rot4.North, this, num * 6f);
    }

    public override void TickAction()
    {
        Thing thing = this;
        if (source != null)
        {
            thing = source;
        }

        var list = new List<Pawn>();
        foreach (var item in Map.mapPawns.AllPawnsSpawned)
        {
            if (item.Position.InHorDistOf(Position, 7.9f) && ToolsLibrary_MParmorOnly.IsUnfriendly(thing, item))
            {
                list.Add(item);
            }
        }

        while (list.Count > 0)
        {
            if (Find.TickManager.TicksGame < TickTime + Age)
            {
                HealthUtility.AdjustSeverity(list[^1], HediffDefOf.XFMParmor_ArcticWind, 1f);
            }

            HealthUtility.AdjustSeverity(list[^1], HediffDefOf.XFMParmor_Freeze, 0.25f);
            list[^1].TakeDamage(new DamageInfo(RimWorld.DamageDefOf.TornadoScratch, 5f, 0f, -1f, thing));
            list.Remove(list[^1]);
        }
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_References.Look(ref source, "skillSource");
    }
}