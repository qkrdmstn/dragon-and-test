using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpawnEditor_BlockSettingUI : MonoBehaviour
{
    public Button saveBtn;
    public Button loadBtn;

    private void Awake()
    {
        saveBtn = transform.GetChild(0).GetComponent<Button>();
        loadBtn = transform.GetChild(1).GetComponent<Button>();
    }

    public void SaveBtnEvent(GameObject loadingUI)
    {
        SpawnEditorManager.instance.SaveSpawnData(loadingUI);
    }

    public void LoadBtnEvent()
    {
        SpawnEditorManager.instance.LoadSpawnData();
    }

    public void ResetBtnEvent()
    {
        SpawnEditorManager.instance.ResetSpawnData();
    }
}
