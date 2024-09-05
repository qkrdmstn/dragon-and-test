using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.EventSystems;

public class SpawnEditor_WaveSettingUI : MonoBehaviour
{
    public Transform CurwaveSetting;

    //WaveInfo
    public Transform waveInfoParent;
    public TextMeshProUGUI waveTxt;
    public Button[] waveBtn;

    //MonsterInfo
    public Transform monsterInfoParent;
    public TextMeshProUGUI monsterTxt;
    public Button[] monsterBtn;

    // Start is called before the first frame update
    void Awake()
    {
        CurwaveSetting = transform.GetChild(0);

        waveInfoParent = CurwaveSetting.GetChild(0);
        waveTxt = waveInfoParent.GetChild(1).GetComponent<TextMeshProUGUI>();
        waveBtn = waveInfoParent.GetComponentsInChildren<Button>();

        monsterInfoParent = CurwaveSetting.GetChild(1);
        monsterTxt = monsterInfoParent.GetChild(1).GetComponent<TextMeshProUGUI>();
        monsterBtn = monsterInfoParent.GetComponentsInChildren<Button>();
    }

    public void WaveBtnEvent(bool increase)
    {
        if (increase)
            SpawnEditorManager.instance.WaveIncrease();
        else
            SpawnEditorManager.instance.WaveDecrease();
        UpdateWaveInfoUI();
    }

    public void MonsterBtnEvent(string monsterType)
    {
        SpawnEditorManager.instance.SetCurMonsterType(monsterType);
        UpdateWaveInfoUI();
    }

    public void UpdateWaveInfoUI()
    {
        monsterTxt.text = SpawnEditorManager.instance.curMonsterType.ToString();
        waveTxt.text = SpawnEditorManager.instance.curWave.ToString();
    }
}
