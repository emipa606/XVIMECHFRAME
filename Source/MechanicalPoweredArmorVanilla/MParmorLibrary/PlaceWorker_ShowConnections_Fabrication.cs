using UnityEngine;
using Verse;

namespace MParmorLibrary;

public class PlaceWorker_ShowConnections_Fabrication : PlaceWorker
{
    public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot, Color ghostCol, Thing thing = null)
    {
        var currentMap = Find.CurrentMap;
        if (def.defName == "XFMParmor_FabricationPit")
        {
            Building_FabricationPit.DrawGrostLine(def, center, rot, currentMap);
        }
        else
        {
            Building_FabricationCentralSystem.DrawGrostLine(def, center, rot, currentMap);
        }
    }
}