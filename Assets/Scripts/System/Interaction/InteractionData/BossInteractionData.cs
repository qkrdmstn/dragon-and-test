using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class BossInteractionData : InteractionData
{
    public bool isActive;
    [SerializeField] CameraManager cameraManager;
    [SerializeField] CinemachineVirtualCamera virtualPlayerCamera;
    [SerializeField] CursorControl cursorControl;
    public int bossDirectionNum;

    public Action[] bossDirectionFuncArray = new Action[4]; // 3개의 함수를 저장할 배열

    private void Start()
    {
        isActive = true;

        bossDirectionFuncArray[0] = BossDirection0;
        bossDirectionFuncArray[1] = BossDirection1;
        bossDirectionFuncArray[2] = BossDirection2;
        bossDirectionFuncArray[3] = BossDirection3;
    }


    public void IsDone()
    {
        isActive=false;
    }

    public void DoBossDirection(int num)
    {
        if (num < 0) return;
        bossDirectionFuncArray[num]();
    }

    void SetCamNoise(float amplitude, float freq)
    {
        CinemachineBasicMultiChannelPerlin perlin = virtualPlayerCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        perlin.m_AmplitudeGain = amplitude;
        perlin.m_FrequencyGain = freq;
    }

    void BossDirection0()
    {
        CursorStop();
    }

    void BossDirection1()
    {
        DialogueInteraction dialogueInteraction = FindAnyObjectByType<DialogueInteraction>();
        dialogueInteraction.SetActiveDialogUI2(true);
        SetCamNoise(0.0f, 0.0f);
    }

    void BossDirection2()
    {
        DialogueInteraction dialogueInteraction = FindAnyObjectByType<DialogueInteraction>();
        dialogueInteraction.SetActiveDialogUI2(false);
        SetCamNoise(1.0f, 1.0f);
    }

    void BossDirection3()
    {
        DialogueInteraction dialogueInteraction = FindAnyObjectByType<DialogueInteraction>();
        dialogueInteraction.SetActiveDialogUI2(false);
        SetCamNoise(2.0f, 1.5f);
    }

    void BossDirection4()
    {

    }
    void BossDirection5()
    {

    }

    void CursorStop()
    {
        cursorControl.SetStopCursor();
    }
    void CursorStart()
    {
        cursorControl.SetStartCursor();
    }

    void CamShakeStop()
    {
        SetCamNoise(0.0f, 0.0f);
    }
}
