using Verse;

namespace MParmorLibrary;

public class Thing_PeriodicAction : ThingWithComps
{
    private int spawnTick;

    public virtual int Cycle => 60;

    public virtual int Age => 120;

    public int TickTime => spawnTick;

    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        base.SpawnSetup(map, respawningAfterLoad);
        spawnTick = Find.TickManager.TicksGame - 1;
        SpawnAction();
    }

    protected override void Tick()
    {
        base.Tick();
        if ((Find.TickManager.TicksGame - spawnTick) % Cycle == 0)
        {
            TickAction();
        }

        if (Find.TickManager.TicksGame < spawnTick + Age)
        {
            return;
        }

        DestroyAction();
        Destroy();
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref spawnTick, "SpawnTick");
    }

    public virtual void SpawnAction()
    {
    }

    public virtual void TickAction()
    {
    }

    public virtual void DestroyAction()
    {
    }
}