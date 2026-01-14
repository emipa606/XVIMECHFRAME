using UnityEngine;
using Verse;

namespace MParmorLibrary;

public class Module : IExposable
{
    public ModuleDef def;

    public MParmorT_ModulesTracker tracker;

    public IMParmorInstance Instance => tracker.instance;

    public MParmorCore Core => Instance as MParmorCore;

    public MParmorBuilding Building => Instance as MParmorBuilding;

    public virtual string TextOnGizmo => null;

    public virtual void ExposeData()
    {
        Scribe_Defs.Look(ref def, "def");
    }

    public virtual void Installed()
    {
    }

    public virtual void TickCore()
    {
    }

    public virtual void TickBuilding()
    {
    }

    public virtual void DrawCore(Vector3 vector3)
    {
    }

    public virtual void DrawBuilding(Vector3 vector3)
    {
    }
}