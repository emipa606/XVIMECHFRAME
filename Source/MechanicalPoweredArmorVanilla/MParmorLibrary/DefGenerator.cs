using Verse;

namespace MParmorLibrary;

[StaticConstructorOnStartup]
public static class DefGenerator
{
    static DefGenerator()
    {
        DefGenerator_NoStaticConstructor.Invoke();
    }
}