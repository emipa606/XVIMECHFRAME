using Verse;

namespace MParmorLibrary;

public class Hediff_MissingPart_NotInstalled : Hediff_MissingPart
{
    public override string LabelBase => def.label;

    public override float SummaryHealthPercentImpact => 0f;

    public override string LabelInBrackets => "";

    public override float BleedRate => 0f;

    public override float PainOffset => 0f;

    public override bool TendableNow(bool ignoreTimer = false)
    {
        return false;
    }
}