using System.Collections.Generic;
using Verse;

namespace MParmorLibrary;

public class CompProperties_LayerDrawer : CompProperties
{
    public List<GraphicDataWithLayer> graphics;

    public CompProperties_LayerDrawer()
    {
        compClass = typeof(CompLayerDrawer);
    }
}