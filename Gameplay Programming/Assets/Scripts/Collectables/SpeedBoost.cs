using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBoost : Collectable
{
    public override void Pickup()
    {
        player.move_speed = RPGCharacterController.base_move_speed * 2;
    }

    public override void Disable()
    {
        player.move_speed = RPGCharacterController.base_move_speed;
    }
}
