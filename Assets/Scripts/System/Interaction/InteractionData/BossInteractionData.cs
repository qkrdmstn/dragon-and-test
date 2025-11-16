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
    [SerializeField] CinemachineImpulseSource impulseSource;
    [SerializeField] CinemachineImpulseListener impulseListener;
    [SerializeField] CinemachineVirtualCamera virtualPlayerCamera;
    [SerializeField] CamShakeProfile quakeProfile;
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
        bossDirectionFuncArray[num]();
    }

    void BossDirection0()
    {

    }

    void BossDirection1()
    {

    }
    void BossDirection2()
    {

    }
    void BossDirection3()
    {

    }
}
