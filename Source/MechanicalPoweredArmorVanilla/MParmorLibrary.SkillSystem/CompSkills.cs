using System.Collections.Generic;
using RimWorld;
using Verse;

namespace MParmorLibrary.SkillSystem;

public class CompSkills : ThingComp
{
    private const float TotalChargingPower = 1040f;
    public List<SkillObject> skills = [];

    public Pawn Wearer => ParentHolder is not Pawn_ApparelTracker { pawn: var pawn } ? null : pawn;

    public MechanicalArmorDef MParmor => ((MParmorCore)parent).System;

    public CompProperties_Skills Props => props as CompProperties_Skills;

    public MParmorCore Core => parent as MParmorCore;

    public void ResetAllSkills()
    {
        skills.Clear();
        for (var i = 0; i < Props.skills.Count; i++)
        {
            skills.Add(new SkillObject
            {
                num = i,
                skill = Props.skills[i],
                parent = this
            });
            skills[i].ResetCaster();
        }
    }

    public override void PostPostMake()
    {
        base.PostPostMake();
        ResetAllSkills();
    }

    public override void PostExposeData()
    {
        base.PostExposeData();
        Scribe_Collections.Look(ref skills, "skills", LookMode.Deep);
        if (skills.NullOrEmpty() || skills.Count < Props.skills.Count)
        {
            PostPostMake();
            return;
        }

        for (var i = 0; i < Props.skills.Count; i++)
        {
            if (skills[i] == null || Props.skills[i] == null)
            {
                continue;
            }

            skills[i].num = i;
            skills[i].skill = Props.skills[i];
            skills[i].parent = this;
            skills[i].ResetCaster();
        }
    }

    public override void CompTick()
    {
        base.CompTick();
        if (Core == null)
        {
            return;
        }

        var num = 0;
        var num2 = 0;
        foreach (var skill in skills)
        {
            if (skill.NeedChagre)
            {
                num++;
            }

            if (skill.CanCharging)
            {
                num2++;
            }

            skill.Tick();
        }

        if (num2 > 0)
        {
            Core.PowerTracker.ConsumeBatteryExactly((int)(41.6f * num2 / num));
        }
    }

    public SkillObject FindSkill(string id)
    {
        foreach (var skill in skills)
        {
            if (skill.skill.id == id)
            {
                return skill;
            }
        }

        return null;
    }

    public override IEnumerable<Gizmo> CompGetWornGizmosExtra()
    {
        if (Core != null)
        {
            foreach (var extraSkillGizmo in Core.GetExtraSkillGizmos())
            {
                yield return extraSkillGizmo;
            }
        }

        foreach (var skill in skills)
        {
            foreach (var item in skill.GetGizmoPetSkill())
            {
                yield return item;
            }
        }
    }
}