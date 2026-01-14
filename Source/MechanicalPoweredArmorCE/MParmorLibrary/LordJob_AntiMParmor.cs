using RimWorld;
using Verse;
using Verse.AI.Group;

namespace MParmorLibrary;

public class LordJob_AntiMParmor : LordJob
{
    public bool ready;

    public override bool GuiltyOnDowned => true;

    public override StateGraph CreateGraph()
    {
        var stateGraph = new StateGraph();
        LordToil lordToil = new LordToil_DestructMParmor
        {
            useAvoidGrid = true
        };
        var transition = new Transition(lordToil, lordToil, true);
        transition.AddTrigger(new Trigger_PawnLost());
        stateGraph.AddTransition(transition);
        LordToil lordToil2 = new LordToil_AntiMParmorFight
        {
            useAvoidGrid = true
        };
        if (ToolsLibrary_MParmorOnly.GetMParmor(Map).Any())
        {
            stateGraph.AddToil(lordToil2);
            stateGraph.AddToil(lordToil);
        }
        else
        {
            stateGraph.AddToil(lordToil);
            stateGraph.AddToil(lordToil2);
        }

        var transition2 = new Transition(lordToil2, lordToil2, true);
        transition2.AddTrigger(new Trigger_PawnLost());
        stateGraph.AddTransition(transition2);
        var transition3 = new Transition(lordToil, lordToil2, true);
        transition3.AddTrigger(new Trigger_AppearActiveMParmor());
        stateGraph.AddTransition(transition3);
        var transition4 = new Transition(lordToil2, lordToil, true);
        transition4.AddTrigger(new Trigger_NonActiveMParmor());
        stateGraph.AddTransition(transition4);
        var lordToil_ExitMapAndDefendSelf = new LordToil_ExitMapAndDefendSelf
        {
            useAvoidGrid = true
        };
        stateGraph.AddToil(lordToil_ExitMapAndDefendSelf);
        var transition5 = new Transition(lordToil, lordToil_ExitMapAndDefendSelf);
        transition5.AddPreAction(new TransitionAction_Message("XFMParmor_LordJob_AntiMParmor".Translate()));
        transition5.AddTrigger(new Trigger_NonMParmor());
        stateGraph.AddTransition(transition5);
        return stateGraph;
    }

    public override void ExposeData()
    {
        Scribe_Values.Look(ref ready, "ready");
    }
}