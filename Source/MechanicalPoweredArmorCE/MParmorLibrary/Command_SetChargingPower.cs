using UnityEngine;
using Verse;

namespace MParmorLibrary;

public class Command_SetChargingPower : Command
{
    public IChargingEquipment equipment;

    public override void ProcessInput(Event ev)
    {
        base.ProcessInput(ev);
        var window = new Dialog_Slider(textGetter, 1, 18, delegate(int value) { equipment.ChargingPower = value; },
            equipment.ChargingPower);
        Find.WindowStack.Add(window);
        return;

        static string textGetter(int x)
        {
            return "XFMParmor_Command_SetChargingPower".Translate(x * 200);
        }
    }
}