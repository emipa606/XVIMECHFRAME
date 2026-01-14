using Verse;

namespace MParmorLibrary.SingleObject;

public static class SingleObjectLibrary
{
    public static void NewExposeData()
    {
        AcquisitionManagement.instance ??= new AcquisitionManagement();

        Scribe_Deep.Look(ref AcquisitionManagement.instance, "XFMParmor_AcquisitionManagement");
    }
}