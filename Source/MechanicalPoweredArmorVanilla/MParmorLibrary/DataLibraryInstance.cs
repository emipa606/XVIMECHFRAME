using RimWorld;

namespace MParmorLibrary;

[DefOf]
public static class DataLibraryInstance
{
    public static DataLibraryDef DataLibrary;

    static DataLibraryInstance()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(DataLibraryInstance));
    }
}