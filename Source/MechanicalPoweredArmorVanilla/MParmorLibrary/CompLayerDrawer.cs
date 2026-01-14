using System.Collections.Generic;
using RimWorld;
using Verse;

namespace MParmorLibrary;

public class CompLayerDrawer : ThingComp
{
    public CompProperties_LayerDrawer Props => (CompProperties_LayerDrawer)props;

    public override void PostDraw()
    {
        IEnumerable<GraphicDataWithLayer> graphics = Props.graphics;
        foreach (var item in graphics ?? [])
        {
            ToolsLibrary.DrawGraphicWithLayer(item.graphicData, parent.TrueCenter(), item.extraRotation,
                parent.Rotation, item.altitudeLayer, null, null, null, parent.def, parent);
        }
    }
}