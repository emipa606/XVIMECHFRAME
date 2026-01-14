using Verse;

namespace MParmorLibrary.Settings;

public class Setting : ModSettings
{
    public bool forceDrafted = true;
    public bool forceFriendlyFire;

    public bool getOutAfterDrafted = true;

    public bool putWeaponIntoInventory = true;

    public static Setting Settings => Main.Instance.settings;

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref forceFriendlyFire, "forceFriendlyFire");
        Scribe_Values.Look(ref forceDrafted, "forceDrafted", true);
        Scribe_Values.Look(ref getOutAfterDrafted, "getOutAfterDrafted", true);
        Scribe_Values.Look(ref putWeaponIntoInventory, "putWeaponIntoInventory", true);
    }
}