using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class JokboUIGroup : UIGroup {

    public GameObject[] hwatuInfoPages;
    //public GameObject[] hwatus;
    [SerializeField] SynergyInfo synergyTable;

    bool isFirst = true;

    private void Awake()
    {
        //hwatus = new GameObject[3];
        SetSynergyName();
    }

    private void Update()
    {
        if (ScenesManager.instance.GetSceneNum() == 0 || !Tutorial.getJokbo) return;
        // start이거나 튜토리얼에서 족보를 아직 획득하지 않았다면 열리지 않습니다.

        if (Input.GetKeyDown(KeyCode.K))
        {
            SetUI();
            JokboState(!childUI[0].activeSelf); // 현재 족보 상태에 따라 열고 닫습니다.
        }   // activeSelf = true : close / false : open
        if (childUI[0].activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            SetUI();
            JokboState(!childUI[0].activeSelf);
        }
    }

    void SetUI()
    {
        if (isFirst)
        {   
            InitDesc();
            isFirst = false;
        }

        if (!childUI[0].activeSelf)
        {   
            foreach (GameObject gameObject in hwatuInfoPages)
            {
                gameObject.SetActive(false);
            }
            childUI[1].transform.GetChild(0).gameObject.SetActive(false);   // back
            childUI[1].transform.GetChild(1).gameObject.SetActive(true);   // exit
            childUI[2].SetActive(true);
        }
    }

    public void InitDesc()
    {
        int idx = 0;
        foreach(GameObject obj in hwatuInfoPages)
        {   // ?? ??? ??? ?? ??
            TextMeshProUGUI[] childobjs = obj.GetComponentsInChildren<TextMeshProUGUI>(true);
            
            if (childobjs.Length == 0) continue;
            for(int i=0; i<childobjs.Length;i+=2)
            {
                childobjs[i].text = synergyTable.SynergyEntity[idx].synergyName;    // name
                childobjs[i + 1].text = synergyTable.SynergyEntity[idx].info;       // synergtDesc
                idx++;
            }

            // ?? ?? ??? ??
            Image[] imgs = obj.GetComponentsInChildren<Image>(true);
            Debug.Log(imgs.Length);
            for (int i = 0; i < imgs.Length; i += 3)
            {
                if (imgs[i].transform.name == "Viewport") i++;
                if (imgs[i].transform.name == "Scrollbar Vertical") break;
                if (imgs[i].transform.name.Contains("KK")) break;

                int synergeType = GetSynergeName(imgs[i].transform.name);

                SeotdaHwatuName[] cards = Hwatu.GetHwatuCombination((SeotdaHwatuCombination)synergeType);

                for (int j = 0; j < SkillManager.instance.hwatuData.Length; j++)
                {
                    if (cards[0] == SkillManager.instance.hwatuData[j].hwatu.type)
                        imgs[i + 1].sprite = SkillManager.instance.hwatuData[j].sprite;

                    else if (cards[1] == SkillManager.instance.hwatuData[j].hwatu.type)
                        imgs[i + 2].sprite = SkillManager.instance.hwatuData[j].sprite;
                }
            }
        }
    }

    string[] synergeName;
    void SetSynergyName() {
        synergeName = new string[22];
        synergeName[0] = SeotdaHwatuCombination.GTT38.ToString();
        synergeName[1] = SeotdaHwatuCombination.GTT18.ToString();
        synergeName[2] = SeotdaHwatuCombination.GTT13.ToString();
        synergeName[3] = SeotdaHwatuCombination.JTT.ToString();
        synergeName[4] = SeotdaHwatuCombination.TT9.ToString();
        synergeName[5] = SeotdaHwatuCombination.TT8.ToString();
        synergeName[6] = SeotdaHwatuCombination.TT7.ToString();
        synergeName[7] = SeotdaHwatuCombination.TT6.ToString();
        synergeName[8] = SeotdaHwatuCombination.TT5.ToString();
        synergeName[9] = SeotdaHwatuCombination.TT4.ToString();
        synergeName[10] = SeotdaHwatuCombination.TT3.ToString();
        synergeName[11] = SeotdaHwatuCombination.TT2.ToString();
        synergeName[12] = SeotdaHwatuCombination.TT1.ToString();
        synergeName[13] = SeotdaHwatuCombination.AL12.ToString();
        synergeName[14] = SeotdaHwatuCombination.DS14.ToString();
        synergeName[15] = SeotdaHwatuCombination.GPP19.ToString();
        synergeName[16] = SeotdaHwatuCombination.JPP110.ToString();
        synergeName[17] = SeotdaHwatuCombination.JS410.ToString();
        synergeName[18] = SeotdaHwatuCombination.SR46.ToString();
        synergeName[19] = SeotdaHwatuCombination.AHES74.ToString();
        synergeName[20] = SeotdaHwatuCombination.TTCatch73.ToString();
        synergeName[21] = SeotdaHwatuCombination.MTGR94.ToString();
    }

    int GetSynergeName(string curSynergy)
    {
        for(int i=0; i<synergeName.Length;i++)
        {
            if (curSynergy == synergeName[i]) return i;
        }

        return -1;
    }

    public void JokboState(bool state)
    {   // true : open / false : close
        GameManager.instance.player.isStateChangeable = !state;
        GameManager.instance.player.isAttackable = !state;
        GameManager.instance.player.isInteraction = state;
        
        Time.timeScale = state ? 0.0f : 1.0f;
        childUI[0].SetActive(state);

        if (!Tutorial.closeJokbo && !state) Tutorial.closeJokbo = true;
    }
}
