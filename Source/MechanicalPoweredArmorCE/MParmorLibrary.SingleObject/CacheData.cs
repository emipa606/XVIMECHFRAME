namespace MParmorLibrary.SingleObject;

public class CacheData
{
    private static readonly CacheData instance = new();

    private CacheData()
    {
    }

    public static CacheData GetInstance()
    {
        return instance;
    }
}