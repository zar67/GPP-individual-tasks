using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineTrigger : MonoBehaviour
{
    public SplineGameplayController target;
    public bool position_start;

    private void OnTriggerEnter(Collider other)
    {
        if (target.triggered)
        {
            target.EndSplineGameplay();
        }
        else
        {
            target.StartSplineGameplay(position_start);
        }
    }
}
