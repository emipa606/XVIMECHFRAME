using System.Collections.Generic;
using Verse;
using Verse.AI.Group;

namespace MParmorLibrary;

public class LordToilData_AntiMParmorFight : LordToilData
{
    public readonly List<Pawn> protectedTarget = [];
    public Thing driver;

    public int lastSpawnShieldTime = -1;

    public Pawn rocketer;

    public List<Pawn> rocketersBackUp = [];

    public ShieldObstacle shield;

    public Pawn shielder;

    public override void ExposeData()
    {
        Scribe_References.Look(ref driver, "driver");
        Scribe_References.Look(ref rocketer, "driver");
        Scribe_References.Look(ref shielder, "shielder");
        Scribe_Collections.Look(ref rocketersBackUp, "rocketersBackUp", LookMode.Reference);
        Scribe_Values.Look(ref lastSpawnShieldTime, "lastSpawnShieldTime", -1);
        Scribe_References.Look(ref shield, "shield");
    }
}