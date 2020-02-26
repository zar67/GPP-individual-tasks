using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineEnterTrigger : MonoBehaviour
{
    public SplineGameplayController target;

    private void OnTriggerEnter(Collider other)
    {
        target.StartSplineGameplay();
    }
}
