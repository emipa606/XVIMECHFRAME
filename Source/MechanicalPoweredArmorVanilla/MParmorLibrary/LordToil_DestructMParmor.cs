using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace MParmorLibrary;

public class LordToil_DestructMParmor : LordToil
{
    public LordToilData_DestructMParmor Data
    {
        get
        {
            data ??= new LordToilData_DestructMParmor();

            return (LordToilData_DestructMParmor)data;
        }
    }

    public override bool ForceHighStoryDanger => true;

    public override bool AllowSatisfyLongNeeds => false;

    public override void Notify_BuildingDespawnedOnMap(Building b)
    {
        if (b is MParmorBuilding)
        {
            UpdateAllDuties();
        }
    }

    public override void UpdateAllDuties()
    {
        if (!lord.ownedPawns.Any())
        {
            return;
        }

        var list = new List<Pawn>();
        list.AddRange(lord.ownedPawns);
        var list2 = MParmorBuilding.Cache.Where((Thing x) => x.Map == Map).ToList();
        if (!list2.Any())
        {
            return;
        }

        foreach (var item in lord.ownedPawns.Where(x => x.equipment.PrimaryEq.PrimaryVerb.IsMeleeAttack))
        {
            Data.destructer = item;
        }

        Data.destructer ??= lord.ownedPawns.RandomElement();

        if (Data.target is not { Spawned: true })
        {
            list2.SortBy(x => ToolsLibrary.GetDistance(x.Position, Data.destructer.Position));
            Data.target = list2.First();
        }

        list.Remove(Data.destructer);
        Data.destructer.mindState.duty = new PawnDuty(DutyDefOf.XFMParmor_Destructer, Data.target);
        foreach (var item2 in list)
        {
            item2.mindState.duty = new PawnDuty(DutyDefOf.XFMParmor_DestructerFollower, Data.target, Data.destructer);
        }
    }
}