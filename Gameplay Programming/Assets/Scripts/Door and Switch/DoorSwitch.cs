using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoorSwitch : Switch
{
    public override bool Triggered()
    {
        return !target.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Default");
    }

    public override bool ReleaseButton()
    {
        return clicked && target.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Close");
    }
}