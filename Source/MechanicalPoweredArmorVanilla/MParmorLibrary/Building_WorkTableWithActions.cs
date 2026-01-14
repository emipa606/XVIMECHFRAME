using System.Collections.Generic;
using RimWorld;
using Verse;

namespace MParmorLibrary;

public abstract class Building_WorkTableWithActions : Building_WorkTable
{
    public abstract void FinishBill(RecipeDef recipeDef, Pawn worker, List<Thing> ingredients);
}