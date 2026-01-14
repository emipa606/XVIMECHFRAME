using System.Collections.Generic;
using RimWorld;
using Verse;

namespace MParmorLibrary;

public class ThingComp_SpecialDisplayStats : ThingComp
{
    public IEnumerable<StatDrawEntry> StatDrawEntry => base.SpecialDisplayStats();
}