using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    //public static CameraManager instance;

    [SerializeField] float globalShakeForce;
    [SerializeField] CinemachineImpulseListener impulseListener;

    private CinemachineImpulseDefinition impulseDefinition;

    private void Awake()
    {
        //if(instance == null)
        //{
        //    instance = this;
        //}
        //else if (instance != this)
        //{ //이미 생성되어 있으면
        //    Destroy(this.gameObject); //새로만든거 삭제
        //}

        globalShakeForce = 0.25f;
        //DontDestroyOnLoad(this.gameObject); //씬이 넘어가도 오브젝트 유지
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
}
