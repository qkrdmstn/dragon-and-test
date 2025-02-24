using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] float globalShakeForce;
    [SerializeField] CinemachineImpulseListener impulseListener;
    [SerializeField] CinemachineConfiner confiner;

    private CinemachineImpulseDefinition impulseDefinition;
   
    private void Awake()
    {
        globalShakeForce = 0.25f;
    }

    public void UpdateConfineArea(PolygonCollider2D coll)
    {
        confiner.m_BoundingShape2D = coll;
    }

    public void CameraShake(CinemachineImpulseSource impulseSource)
    {   // global type by object
        impulseSource.GenerateImpulseWithForce(globalShakeForce);
    }

    public void CameraShakeFromProfile(CamShakeProfile profile, CinemachineImpulseSource impulseSource)
    {   // custom type by object
        SetCameraShakeProfile(profile, impulseSource);
        impulseSource.GenerateImpulseWithForce(profile.impactForce);
    }


    void SetCameraShakeProfile(CamShakeProfile profile, CinemachineImpulseSource impulseSource)
    {
        impulseDefinition = impulseSource.m_ImpulseDefinition;

        // Custom Impulse Source Setting
        impulseDefinition.m_ImpulseDuration = profile.impactDuration;
        impulseDefinition.m_CustomImpulseShape = profile.impulseCurve;

        impulseSource.m_DefaultVelocity = profile.defaultVelocity;

        // Custom Impulse Listener Setting
        impulseListener.m_ReactionSettings.m_AmplitudeGain = profile.listenerAmplitude;
        impulseListener.m_ReactionSettings.m_FrequencyGain = profile.listenerFrequency;
        impulseListener.m_ReactionSettings.m_Duration = profile.listenerDuration;
    }
    public void CameraShakeFromProfile(CamShakeProfile profile, CinemachineImpulseSource impulseSource, Vector3 velocity)
    {   // custom type by object
        SetCameraShakeProfile(profile, impulseSource, velocity);
        impulseSource.GenerateImpulseWithForce(profile.impactForce);
    }

    void SetCameraShakeProfile(CamShakeProfile profile, CinemachineImpulseSource impulseSource, Vector3 velocity)
    {
        impulseDefinition = impulseSource.m_ImpulseDefinition;

        // Custom Impulse Source Setting
        impulseDefinition.m_ImpulseDuration = profile.impactDuration;
        impulseDefinition.m_CustomImpulseShape = profile.impulseCurve;

        impulseSource.m_DefaultVelocity = velocity;

        // Custom Impulse Listener Setting
        impulseListener.m_ReactionSettings.m_AmplitudeGain = profile.listenerAmplitude;
        impulseListener.m_ReactionSettings.m_FrequencyGain = profile.listenerFrequency;
        impulseListener.m_ReactionSettings.m_Duration = profile.listenerDuration;
    }
}
