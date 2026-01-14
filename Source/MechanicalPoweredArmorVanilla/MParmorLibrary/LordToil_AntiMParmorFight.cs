using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace MParmorLibrary;

public class LordToil_AntiMParmorFight : LordToil
{
    private static List<Pawn> pawns;

    private static List<Pawn> members;

    private static List<Pawn> antiShield;

    public LordToilData_AntiMParmorFight Data
    {
        get
        {
            data ??= new LordToilData_AntiMParmorFight();

            return (LordToilData_AntiMParmorFight)data;
        }
    }

    public override bool ForceHighStoryDanger => true;

    public override bool AllowSatisfyLongNeeds => false;

    public override void LordToilTick()
    {
        CheckShield();
        CheckTarget();
        CheckRocketer();
    }

    private void CheckShield()
    {
        if (Data.shielder != null &&
            (Data.shield == null || Find.TickManager.TicksGame - Data.lastSpawnShieldTime > 420))
        {
            UpdateAllDuties();
        }
    }

    private void CheckRocketer()
    {
        if (Data.rocketersBackUp.Count <= 0 || lord.ticksInToil - 2400 <= 0 || (lord.ticksInToil - 2400) % 900 != 0)
        {
            return;
        }

        CheckRocketersBackUp();
        if (Data.rocketersBackUp.Any() && Data.rocketer == null)
        {
            Data.rocketer = Data.rocketersBackUp.RandomElement();
        }
    }

    private void CheckRocketersBackUp()
    {
        var list = new List<Pawn>();
        foreach (var item in Data.rocketersBackUp)
        {
            if (IsRocketer(item))
            {
                continue;
            }

            list.Add(item);
            if (Data.rocketer == item)
            {
                Data.rocketer = null;
            }
        }

        while (list.Count > 0)
        {
            Data.rocketersBackUp.Remove(list[0]);
            list.RemoveAt(0);
        }
    }

    private void CheckTarget()
    {
        if (Data.driver is not { Spawned: true })
        {
            UpdateAllDuties();
        }
        else if (lord.ticksInToil % 600 == 0 && (Data.driver is not Pawn pawn || !pawn.GetMParmorCore()))
        {
            UpdateAllDuties();
        }
    }

    public override void UpdateAllDuties()
    {
        (pawns ?? (pawns = [])).Clear();
        (members ?? (members = [])).Clear();
        (antiShield ?? (antiShield = [])).Clear();
        Data.rocketersBackUp.Clear();
        Data.protectedTarget.Clear();
        Data.shielder = null;
        pawns.AddRange(lord.ownedPawns.Where(x => x.Spawned));
        if (!pawns.Any())
        {
            return;
        }

        members.AddRange(pawns);
        var driver = GetDriver();
        Data.driver = driver;
        foreach (var pawn in pawns)
        {
            if (pawn.equipment.Primary.def.weaponTags.Contains("XFMParmor_AntiMPArmorMelee"))
            {
                pawn.mindState.duty = new PawnDuty(DutyDefOf.XFMParmor_AntiMParmor_Melee, driver);
                members.Remove(pawn);
            }
            else if (pawn.equipment.Primary.def.weaponTags.Contains("XFMParmor_AntiMPArmorLong"))
            {
                pawn.mindState.duty = new PawnDuty(DutyDefOf.XFMParmor_AntiMParmor_LongDistance, driver);
                members.Remove(pawn);
            }
            else if (pawn.equipment.Primary.def.weaponTags.Contains("破盾"))
            {
                members.Remove(pawn);
                antiShield.Add(pawn);
            }
            else if (IsRocketer(pawn))
            {
                members.Remove(pawn);
                Data.rocketersBackUp.Add(pawn);
            }
        }

        Data.protectedTarget.AddRange(members);
        Data.protectedTarget.AddRange(antiShield);
        Data.protectedTarget.AddRange(Data.rocketersBackUp);
        var leader = GetLeader(members, driver);
        members.Remove(leader);
        foreach (var member in members)
        {
            member.mindState.duty = new PawnDuty(DutyDefOf.XFMParmor_AntiMParmor_Standard, driver, leader);
        }

        foreach (var item in Data.rocketersBackUp)
        {
            item.mindState.duty = new PawnDuty(DutyDefOf.XFMParmor_AntiMParmor_Rocket, driver, leader);
        }
    }

    private static bool IsRocketer(Pawn pawn)
    {
        if (pawn.equipment.Primary.def.defName == "XFMParmor_RocketLauncher")
        {
            return true;
        }

        foreach (var item in pawn.inventory.GetDirectlyHeldThings())
        {
            if (item.def.defName == "XFMParmor_RocketLauncher")
            {
                return true;
            }
        }

        return false;
    }

    private Thing GetDriver()
    {
        if (TryGetOrginDriver(out var thing))
        {
            return thing;
        }

        var mParmor = ToolsLibrary_MParmorOnly.GetMParmor(Map);
        return mParmor.First().Wearer;
    }

    private bool TryGetOrginDriver(out Thing thing)
    {
        thing = null;
        if (Data.driver == null)
        {
            return false;
        }

        if (Data.driver.Spawned)
        {
            thing = Data.driver;
            return true;
        }

        if (Data.driver.ParentHolder is not Thing { Spawned: not false } thing2)
        {
            return false;
        }

        thing = thing2;
        return true;
    }

    private Pawn GetLeader(List<Pawn> members, Thing driver)
    {
        Pawn pawn = null;
        using (var enumerator = members.Where(CanBeLeaderFirstly).GetEnumerator())
        {
            if (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                Data.shielder = current;
                pawn = current;
            }
        }

        pawn ??= !members.Any() ? pawns.RandomElement() : members.RandomElement();

        pawn.mindState.duty = new PawnDuty(DutyDefOf.XFMParmor_AntiMParmor_Leader, driver);
        return pawn;

        static bool CanBeLeaderFirstly(Pawn pawn2)
        {
            foreach (var item in pawn2.apparel.WornApparel)
            {
                if (item.def.defName == "XFMParmor_AntiMParmor_ShieldSpawner")
                {
                    return true;
                }
            }

            return false;
        }
    }
}