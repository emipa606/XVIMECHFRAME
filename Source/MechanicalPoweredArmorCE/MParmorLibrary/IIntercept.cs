using CombatExtended;
using Verse;

namespace MParmorLibrary;

public interface IIntercept
{
    bool CanIntercept(Projectile projectile, IntVec3 c);

    bool TryIntercept(Projectile projectile, IntVec3 c);

    bool CanIntercept(ProjectileCE projectile, IntVec3 c);

    bool TryIntercept(ProjectileCE projectile, IntVec3 c);
}