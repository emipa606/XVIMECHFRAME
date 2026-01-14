using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace MParmorLibrary;

public class CompLayerDrawer_Wall_SelfRepairing : ThingComp
{
    private CompProperties_LayerDrawer Props => (CompProperties_LayerDrawer)props;

    public override void PostDraw()
    {
        IEnumerable<GraphicDataWithLayer> graphics = Props.graphics;
        foreach (var item in graphics ?? [])
        {
            Color? color = null;
            if (parent is Wall_SelfRepairing wall_SelfRepairing)
            {
                color = wall_SelfRepairing.HitPointColor;
            }

            ToolsLibrary.DrawGraphicWithLayer(item.graphicData, parent.TrueCenter(), item.extraRotation,
                parent.Rotation, item.altitudeLayer, color, null, null, parent.def, parent);
        }
    }
}