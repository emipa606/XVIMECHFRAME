using System.Collections.Generic;
using Verse;

namespace MParmorLibrary;

public class DataLibraryDef : Def
{
    [MustTranslate] public List<string> forceFriendlyFire;

    public List<ThingDef> weaponDef_AntiMachine;

    public List<ThingDef> weaponDef_AntiShield;
    public List<ThingDef> weaponDef_IgnoreShield;
}