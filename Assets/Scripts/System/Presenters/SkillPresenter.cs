using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum ActiveSkillSlot { Q, E }

[Serializable]
public struct ActiveData
{   // active
    public Image skillImg;
    public ActiveSkillSlotUI activeSkillSlot;
    // cooltime
    public GameObject coolTimeObj;
    public Image coolTimeImg;
    public TextMeshProUGUI coolTimeTxt;
}

[Serializable]
public struct PassiveData
{
    public Image skillImg;
    public TextMeshProUGUI skillInfoTxt;
    public PassiveSkillSlotUI passiveSkillSlot; // data로 섯다 조합 확인 가능
}

public class SkillPresenter : PresenterBase
{
    [Header("Model")]
    public SkillManager m_Skill;

    [Header("View")]
    public SerializableDictionary<ActiveSkillSlot, ActiveData> activeUI;
    public List<PassiveData> passiveUI;
    Dictionary<SeotdaHwatuCombination, int> passiveIdxTable;    // 추가할 때마다 인덱스 조정, 패시브 삭제는 미존재
    int curAddedIdx = 0;    

    void Awake()
    {
        InitializeUI();
    }

    #region ActionSetting
    void Start()
    {
        m_Skill.active.action += ActiveSkillChanged;
        m_Skill.active.action += CoolTimeChanged;

        m_Skill.passive.action += PassiveSkillChanged;
    }

    void OnDestroy()
    {
        m_Skill.active.action -= ActiveSkillChanged;
        m_Skill.active.action -= CoolTimeChanged;

        m_Skill.passive.action -= PassiveSkillChanged;
    }
    #endregion

    void InitializeUI()
    {
        passiveIdxTable = new Dictionary<SeotdaHwatuCombination, int>();
        foreach (ActiveSkillSlot skillSlot in activeUI.Keys)
        {
            ClearActiveSlot(skillSlot);
            activeUI[skillSlot].coolTimeObj.SetActive(false);
        }
    }
    #region ActiveSkillSlot
    public void ActiveSkillChanged(ActiveSkillSlot slot, bool checkCoolTime)
    {
        ActiveSkillSlot skillSlot = (ActiveSkillSlot)Enum.ToObject(typeof(ActiveSkillSlot), slot);
        UpdateActiveSkillSlot(skillSlot);
    }

    void UpdateActiveSkillSlot(ActiveSkillSlot skillSlot)
    { // clear -> update
        ClearActiveSlot(skillSlot);
        UpdateSkillSlotView(skillSlot);
    }

    void UpdateSkillSlotView(ActiveSkillSlot skillSlot)
    {   // skillslot에 따른 변경 스킬 UI 갱신
        SeotdaHwatuCombination _data = m_Skill.active[skillSlot].combination;
        if (_data != SeotdaHwatuCombination.blank)
        {
            activeUI[skillSlot].activeSkillSlot.data = _data;
            activeUI[skillSlot].skillImg.color = Color.white;
            activeUI[skillSlot].skillImg.sprite = m_Skill.skillSpriteDictionary[_data];
        }
        else
        {
            activeUI[skillSlot].activeSkillSlot.data = SeotdaHwatuCombination.blank;
            activeUI[skillSlot].skillImg.color = Color.clear;
            activeUI[skillSlot].skillImg.sprite = null;
        }
    }

    void ClearActiveSlot(ActiveSkillSlot skillSlot)
    {
        activeUI[skillSlot].skillImg.sprite = null;
        activeUI[skillSlot].skillImg.color = Color.clear;
    }
    #endregion

    #region PassiveSkillSlot
    public void PassiveSkillChanged(int combination)
    {
        SeotdaHwatuCombination _data = (SeotdaHwatuCombination)Enum.ToObject(typeof(SeotdaHwatuCombination), combination);
        int idx = FindPassiveSkillIdx(_data);
        if (idx == -1)
            idx = AddPassiveIdx(_data);

        UpdatePassiveSkillSlot(_data, idx);
    }

    int FindPassiveSkillIdx(SeotdaHwatuCombination _data)
    {
        if (passiveIdxTable.ContainsKey(_data))
            return passiveIdxTable[_data];
        else return -1;
    }

    int AddPassiveIdx(SeotdaHwatuCombination _data)
    {
        passiveIdxTable.Add(_data, curAddedIdx);
        return curAddedIdx++;
    }

    void UpdatePassiveSkillSlot(SeotdaHwatuCombination _data, int idx)
    {
        ClearPassiveSlot(idx);
        UpdateSkillInventoryView(_data, idx);
    }

    void UpdateSkillInventoryView(SeotdaHwatuCombination _data, int idx)
    {
        if(_data != SeotdaHwatuCombination.blank)
        {
            passiveUI[idx].passiveSkillSlot.data = _data;
            passiveUI[idx].skillImg.sprite = m_Skill.skillSpriteDictionary[_data];
            passiveUI[idx].skillInfoTxt.text = m_Skill.GetSkillInfo(_data, true);
            passiveUI[idx].skillImg.color = Color.white;
        }
        else
        {
            passiveUI[idx].passiveSkillSlot.data = SeotdaHwatuCombination.blank;
            passiveUI[idx].skillImg.sprite = null;
            passiveUI[idx].skillInfoTxt.text = null;
            passiveUI[idx].skillImg.color = Color.clear;
        }
    }

    void ClearPassiveSlot(int idx)
    {
        passiveUI[idx].skillInfoTxt.text = null;
        passiveUI[idx].skillImg.sprite = null;
        passiveUI[idx].skillImg.color = Color.clear;
    }
    #endregion

    #region CoolTime
    public void CoolTimeChanged(ActiveSkillSlot slot, bool checkCoolTime)
    {
        activeUI[slot].coolTimeObj.SetActive(checkCoolTime);
        if (checkCoolTime)
            StartCoroutine(CoolTimeFunc(slot, m_Skill.skillDBDictionary[m_Skill.active[slot].combination].coolTime));
    }

    IEnumerator CoolTimeFunc(ActiveSkillSlot slot, float originCoolTime)
    {
        while (m_Skill.active[slot].curCoolTime > 0.0f)
        {
            float curCoolTime = m_Skill.active[slot].curCoolTime;
            
            curCoolTime -= Time.deltaTime;
            activeUI[slot].coolTimeImg.fillAmount = curCoolTime / originCoolTime;
            activeUI[slot].coolTimeTxt.text = Math.Round(curCoolTime, 1).ToString();

            m_Skill.active[slot, curCoolTime] = curCoolTime;
            yield return new WaitForFixedUpdate();
        }
        EndCoolTime(slot, originCoolTime);
    }

    void EndCoolTime(ActiveSkillSlot slot, float originTime)
    {
        activeUI[slot].coolTimeObj.SetActive(false);
        m_Skill.ClearCoolTimer(slot, originTime); // slot에 대한 스킬 현재 쿨타임 초기화
    }
    #endregion

    public override bool ActivateEachUI()
    {
        if(!base.ActivateEachUI())
        {   // UI기 켜져야하는 배틀, 튜토리얼, 보스, 퍼즐씬
            objs[1].SetActive(true);    // skillslot
            objs[2].SetActive(true);    // hwatu
        }
        return true;
    }
}
