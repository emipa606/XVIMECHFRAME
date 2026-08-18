using System.Collections.Generic;
using Verse;

namespace MParmorLibrary.SingleObject;

public class ProjectilesStore
{
    private static ProjectilesStore instance = new();

    public List<Projectile> projectiles = [];

    public static ProjectilesStore GetInstance()
    {
        return instance ?? (instance = new ProjectilesStore());
    }
}