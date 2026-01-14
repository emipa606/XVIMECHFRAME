using RimWorld;
using Verse;

namespace MParmorLibrary;

public class CompUseEffect_ForceWear : CompUseEffect
{
    public override void DoEffect(Pawn usedBy)
    {
        usedBy.apparel.Wear((Apparel)parent);
    }
}