using System.Collections.Generic;
using MParmorLibrary.SkillSystem;
using Verse;

namespace MParmorLibrary;

public class ShieldObstacle : Building
{
    public SkillObject holder;

    private Health_Shiled shieldClass;
    public int superShieldTime = 0;

    public float ShieldMax => ShieldClass.ShieldMax;

    private float Shield => ShieldClass.Shield;

    public Health_Shiled ShieldClass
    {
        get
        {
            shieldClass ??= new Health_Shiled();
            return shieldClass;
        }
        set => shieldClass = value;
    }

    protected override void Tick()
    {
        base.Tick();
        if (holder != null && holder.parent.Wearer == null)
        {
            SheildBreak();
        }
        else if (Shield == 0f)
        {
            SheildBreak();
        }
    }

    public virtual void Hurt_Shield(DamageInfo dinfo)
    {
        ShieldClass.Hurt_Shield(dinfo);
    }

    public virtual void SheildBreak()
    {
        holder?.thingHolder = null;

        Destroy();
    }

    public override IEnumerable<Gizmo> GetGizmos()
    {
        foreach (var gizmo in base.GetGizmos())
        {
            yield return gizmo;
        }

        var text = holder == null || holder.parent.Wearer == null
            ? (string)"XFMParmor_Shield".Translate()
            : (string)("XFMParmor_From".Translate() + holder.parent.Wearer.LabelCap);
        foreach (var gizmo2 in ShieldClass.GetGizmos(text))
        {
            yield return gizmo2;
        }
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Deep.Look(ref shieldClass, "shieldClass");
        Scribe_References.Look(ref holder, "holder");
    }
}