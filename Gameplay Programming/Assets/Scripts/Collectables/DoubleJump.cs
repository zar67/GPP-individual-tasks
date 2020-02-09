using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleJump : Collectable
{
    public override void Update()
    {
        base.Update();

        if (player.double_jump == this && (player.set_jump || player.set_double_jump))
        {
            Instantiate(player_particles_prefab, player.gameObject.transform.position, player.gameObject.transform.rotation, player.gameObject.transform);
        }
    }

    public override void Pickup()
    {
        player.double_jump = this;
        player.can_double_jump = true;
    }

    public override void Disable()
    {
        if (player.double_jump == this)
        {
            player.double_jump = null;
            player.can_double_jump = false;
        }
    }
}
