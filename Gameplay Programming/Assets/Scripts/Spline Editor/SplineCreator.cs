using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineCreator : MonoBehaviour
{
    [HideInInspector]
    public Spline path;

    public void CreatePath()
    {
        path = new Spline(transform.position);
    }
}