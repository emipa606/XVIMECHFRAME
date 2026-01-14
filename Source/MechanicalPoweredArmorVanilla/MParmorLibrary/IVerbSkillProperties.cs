namespace MParmorLibrary;

public interface IVerbSkillProperties
{
    bool CanUseNow(out string reason);
}