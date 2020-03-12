using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformCutscene : Cutscene
{
    public override void Trigger()
    {
        target_switch.target.GetComponent<MovingPlatformsController>().StartPlatforms();
    }
}
