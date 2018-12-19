using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smoothstep : MonoBehaviour
{

    // Minimum and maximum values for the transition.
    float minimum = 0.0f;
    float maximum = 5.0f;

    // Time taken for the transition.
    float duration = 5.0f;

    float startTime;

    void Start()
    {
        // Make a note of the time the script started.
        startTime = Time.time;
    }

    void Update()
    {
        // Calculate the fraction of the total duration that has passed.
        float t = (Time.time - startTime);
        transform.position = new Vector3(Mathf.SmoothStep(minimum, maximum, t), 0, 0);
        Debug.Log(Mathf.SmoothStep(minimum, maximum, t) + " - " + t);
    }
}
