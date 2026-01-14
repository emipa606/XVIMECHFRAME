using UnityEngine;
using Verse;

namespace MParmorLibrary.Settings;

[StaticConstructorOnStartup]
public class Main : Mod
{
    public readonly Setting settings;

    public Main(ModContentPack content)
        : base(content)
    {
        settings = GetSettings<Setting>();
        Instance = this;
    }

    public static Main Instance { get; private set; }

    public override string SettingsCategory()
    {
        return "XFMParmor_Settings_SettingsCategory".Translate();
    }

    public override void DoSettingsWindowContents(Rect inRect)
    {
        var listing_Standard = new Listing_Standard();
        listing_Standard.Begin(inRect);
        listing_Standard.GapLine(20f);
        Text.Font = GameFont.Medium;
        listing_Standard.CheckboxLabeled("XFMParmor_Settings_getOutAfterDraftedA".Translate(),
            ref settings.getOutAfterDrafted, "XFMParmor_Settings_getOutAfterDraftedB".Translate());
        listing_Standard.CheckboxLabeled("XFMParmor_Settings_forceDraftedA".Translate(), ref settings.forceDrafted,
            "XFMParmor_Settings_forceDraftedB".Translate());
        listing_Standard.CheckboxLabeled("XFMParmor_Settings_putWeaponIntoInventoryA".Translate(),
            ref settings.putWeaponIntoInventory, "XFMParmor_Settings_putWeaponIntoInventoryB".Translate());
        listing_Standard.CheckboxLabeled("XFMParmor_Settings_forceFriendlyFireA".Translate(),
            ref settings.forceFriendlyFire, "XFMParmor_Settings_forceFriendlyFireB".Translate());
        Text.Font = GameFont.Small;
        foreach (var item in DataLibraryInstance.DataLibrary.forceFriendlyFire)
        {
            listing_Standard.Label(item);
        }

        Text.Font = GameFont.Small;
        listing_Standard.End();
    }
}