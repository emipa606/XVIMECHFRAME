using System.Collections.Generic;
using Verse;

namespace MParmorLibrary;

public static class Intercepts
{
    public static readonly Dictionary<Map, List<IIntercept>> intercepts = new();

    public static void AddNewInstance(Thing thing)
    {
        if (intercepts.TryGetValue(thing.Map, out var value))
        {
            value.Add(thing as IIntercept);
            return;
        }

        intercepts.Add(thing.Map, [thing as IIntercept]);
    }

    public static void RemoveInstance(Thing thing)
    {
        if (intercepts.TryGetValue(thing.Map, out var value))
        {
            value.Remove(thing as IIntercept);
        }
    }
}