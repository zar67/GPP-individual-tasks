using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public DoorController target;

    private void OnTriggerEnter(Collider other)
    {
        target.Open();
    }

    private void OnTriggerExit(Collider other)
    {
        //target.Close();
    }
}
