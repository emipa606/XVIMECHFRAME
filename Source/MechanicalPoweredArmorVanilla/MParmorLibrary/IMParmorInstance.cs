namespace MParmorLibrary;

public interface IMParmorInstance
{
    MechanicalArmorDef System { get; }

    MParmorT_PowerTracker PowerTracker { get; }

    MParmorT_HealthTracker HealthTracker { get; }

    MParmorT_ModulesTracker ModulesTracker { get; }

    void CopyTracker(IMParmorInstance instance);
}