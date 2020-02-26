using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineExitTrigger : MonoBehaviour
{
    public SplineGameplayController target;

    private void OnTriggerEnter(Collider other)
    {
        target.EndSplineGameplay();
    }
}
