using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player") && GetComponentInParent<DoorController>().close_after_walked_through)
        {
            GetComponentInParent<DoorController>().Close();
        }
    }
}
