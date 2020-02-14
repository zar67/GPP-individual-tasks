using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public DoorController target;

    private void OnTriggerEnter(Collider other)
    {
        if (target.close_after_walked_through)
        {
            target.Close();
        }
    }
}
