using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBoost : Collectable
{
    public override void Pickup()
    {
        player.speed_boost = this;
        player.move_speed = RPGCharacterController.base_move_speed * 2;
    }

    public override void Disable()
    {
        if (player.speed_boost == this)
        {
            player.speed_boost = null;
            player.move_speed = RPGCharacterController.base_move_speed;
        }
    }
}
