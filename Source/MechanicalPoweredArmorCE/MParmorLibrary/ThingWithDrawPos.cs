using UnityEngine;
using Verse;

namespace MParmorLibrary;

public class ThingWithDrawPos(Vector3 drawPos) : Thing
{
    public override Vector3 DrawPos => drawPos;
}