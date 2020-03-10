using System.Collections;
using UnityEngine;

public class DoorCutscene : Cutscene
{
    public override void Trigger()
    {
        target_switch.target.Trigger();
    }
}
