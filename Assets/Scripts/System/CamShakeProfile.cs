using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="CamShake/New Profile")]
public class CamShakeProfile : ScriptableObject
{
    [Header("Impulse Source Setting")]
    public float impactDuration = 0.2f;
    public float impactForce = 1f;
    public Vector3 defaultVelocity = new Vector3(0, -1, 0);
    public AnimationCurve impulseCurve;

    [Header("Impulse Listener Setting")]
    public float listenerAmplitude = 1f;
    public float listenerFrequency = 1f;
    public float listenerDuration = 1f;

}
