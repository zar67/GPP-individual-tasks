using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleJump : Collectable
{
    public override void Pickup()
    {
        player.can_double_jump = true;
    }

    public override void Disable()
    {
        player.can_double_jump = false;
    }
}
