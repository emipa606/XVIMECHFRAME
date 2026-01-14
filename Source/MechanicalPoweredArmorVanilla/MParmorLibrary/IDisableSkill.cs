namespace MParmorLibrary;

public interface IDisableSkill
{
    bool DisableNow { get; }

    string DisableReason { get; }
}