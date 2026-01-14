using CombatExtended;

namespace MParmorLibrary;

public class Bullet_BloodIce : BulletCE
{
    public override void Tick()
    {
        if ((DrawPos.V3ToV2() - origin).sqrMagnitude > 49f)
        {
            Destroy();
        }
        else
        {
            Tick();
        }
    }
}