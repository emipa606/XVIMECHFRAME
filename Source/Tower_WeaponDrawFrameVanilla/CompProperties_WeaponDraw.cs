using Verse;

namespace Tower_WeaponDrawFrame;

public class CompProperties_WeaponDraw : CompProperties
{
    public readonly RotOffset rotOffset = new();

    public RotOffset offHand;

    public CompProperties_WeaponDraw()
    {
        compClass = typeof(CompWeaponDraw);
    }
}