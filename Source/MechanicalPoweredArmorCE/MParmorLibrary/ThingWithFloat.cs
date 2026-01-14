using System;
using Verse;

namespace MParmorLibrary;

public class ThingWithFloat(Thing thing, float floatValue) : IComparable
{
    public readonly float floatValue = floatValue;
    public readonly Thing thing = thing;

    public float Distance => (float)Math.Sqrt(floatValue);

    public int CompareTo(object value)
    {
        if (value == null)
        {
            return 1;
        }

        if (value is float value2)
        {
            return CompareTo(value2);
        }

        if (value is ThingWithFloat thingWithFloat)
        {
            return CompareTo(thingWithFloat.floatValue);
        }

        return 0;
    }

    private int CompareTo(float value)
    {
        if (floatValue < value)
        {
            return -1;
        }

        return floatValue > value ? 1 : 0;
    }
}