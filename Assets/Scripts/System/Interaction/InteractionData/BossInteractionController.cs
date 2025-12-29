using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class BossInteractionController : MonoBehaviour
{
    public InteractionData bossStageStartInteraction;
    public InteractionData bossStageClearInteraction;
    public InteractionData bossStageFailInteraction;
    public bool isActive;
    [SerializeField] CameraManager cameraManager;
    [SerializeField] CinemachineVirtualCamera virtualPlayerCamera;
    [SerializeField] CinemachineVirtualCamera virtualBossCamera;
    [SerializeField] CursorControl cursorControl;
    public int bossDirectionNum;

    public Action[] bossDirectionFuncArray = new Action[10]; // 3개의 함수를 저장할 배열

    private void Start()
    {
        isActive = true;

        bossDirectionFuncArray[0] = BossDirection0;
        bossDirectionFuncArray[1] = BossDirection1;
        bossDirectionFuncArray[2] = BossDirection2;
        bossDirectionFuncArray[3] = BossDirection3;
        bossDirectionFuncArray[4] = BossDirection4;
        bossDirectionFuncArray[5] = BossDirection5;
        bossDirectionFuncArray[6] = BossDirection6;
        bossDirectionFuncArray[7] = BossDirection7;
        bossDirectionFuncArray[8] = BossDirection8;
        bossDirectionFuncArray[9] = BossDirection9;
    }

    public void BossFail()
    {
        Debug.Log("!");
        PlayerInteraction playerInteraction = FindObjectOfType<PlayerInteraction>();
        playerInteraction.interaction = bossStageFailInteraction;
        FindObjectOfType<Boss_Jan>().BossPause();
        playerInteraction.DoInteraction();
        StartCoroutine(BossFailCoroutine());
    }

    //패배 대사 출력 완료까지 기다린 뒤, 사망 UI 출력
    public IEnumerator BossFailCoroutine()
    {
        yield return new WaitUntil(() => UIManager.instance.isClose);
        UIManager.instance.SceneUI["Dead"].SetActive(true);
    }

    public void BossClear()
    {
        Debug.Log("!");
        PlayerInteraction playerInteraction = FindObjectOfType<PlayerInteraction>();
        playerInteraction.interaction = bossStageClearInteraction;
        playerInteraction.DoInteraction();
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

    void BossDirection0()
    {
        CursorStop();
    }

    //플레이어 카메라 흔들림 없애기
    void BossDirection1()
    {
        DialogueInteraction dialogueInteraction = FindAnyObjectByType<DialogueInteraction>();
        dialogueInteraction.SetActiveDialogUI2(true);
        SetPlayerCamNoise(0.0f, 0.0f);
    }

    //플레이어 카메라 흔들림 추가 && 대화 UI Inactive
    void BossDirection2()
    {
        DialogueInteraction dialogueInteraction = FindAnyObjectByType<DialogueInteraction>();
        dialogueInteraction.SetActiveDialogUI2(false);
        SetPlayerCamNoise(1.0f, 1.0f);
    }

    //플레이어 카메라 -> 보스 카메라 && 보스 카메라 흔들림 추가
    void BossDirection3()
    {
        DialogueInteraction dialogueInteraction = FindAnyObjectByType<DialogueInteraction>();
        dialogueInteraction.SetActiveDialogUI2(false);
        SetPlayerCamPriority(-5);
        SetBossCamNoise(1.0f, 1.0f);
        SetPlayerCamNoise(1.0f, 1.0f);
    }

    //보스 카메라 심하게 흔들림 && 대화 UI Inactive
    void BossDirection4()
    {
        DialogueInteraction dialogueInteraction = FindAnyObjectByType<DialogueInteraction>();
        dialogueInteraction.SetActiveDialogUI2(false);
        SetBossCamNoise(2.0f, 1.5f);
    }


    void BossDirection5()
    {
        SetPlayerCamPriority(10);
        CursorStart();
    }

    void BossDirection6()
    {
        DialogueInteraction dialogueInteraction = FindAnyObjectByType<DialogueInteraction>();
        dialogueInteraction.SetActiveDialogUI2(true);
    }

    //Boss 카메라 흔들림 없애기
    void BossDirection7()
    {
        DialogueInteraction dialogueInteraction = FindAnyObjectByType<DialogueInteraction>();
        dialogueInteraction.SetActiveDialogUI2(true);
        SetBossCamNoise(0.0f, 0.0f);
        SetPlayerCamNoise(0.0f, 0.0f);
    }

    void BossDirection8()
    {

    }
    void BossDirection9()
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

    void SetPlayerCamNoise(float amplitude, float freq)
    {
        CinemachineBasicMultiChannelPerlin perlin = virtualPlayerCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        perlin.m_AmplitudeGain = amplitude;
        perlin.m_FrequencyGain = freq;
    }

    void SetBossCamNoise(float amplitude, float freq)
    {
        CinemachineBasicMultiChannelPerlin perlin = virtualBossCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        perlin.m_AmplitudeGain = amplitude;
        perlin.m_FrequencyGain = freq;
    }

    void SetPlayerCamPriority(int priority)
    {
        virtualPlayerCamera.Priority = priority;
    }
}
