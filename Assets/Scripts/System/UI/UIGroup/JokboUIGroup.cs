using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public class JokboHwatuUI
{
    public GameObject[] hwatu;
}

public class JokboUIGroup : UIGroup {

    public GameObject[] hwatuInfoPages;
    public JokboHwatuUI[] hwatuUIs;
    public bool isPossibleJokbo = false;
    bool isFirst = true;

    private void Awake()
    {
        SetSynergyName();
    }

    void Update()
    {
        if (!childUI[0].activeSelf && Input.GetKeyDown(KeyCode.K)) // open
            JokboState(true);
        else if (childUI[0].activeSelf && Input.GetKeyDown(KeyCode.Escape))
            JokboState(false);
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
            childUI[2].SetActive(true);
        }
    }

    public void InitDesc()
    {
        int idx = 0;
        for(int k =0; k< hwatuUIs.Length; k++)
        {
            for (int i = 0; i < hwatuUIs[k].hwatu.Length; i++)
            {
                if (k == 3)
                {
                    idx++; continue;
                }

                GameObject hwatu = hwatuUIs[k].hwatu[i];
                TextMeshProUGUI[] childTxts = hwatu.GetComponentsInChildren<TextMeshProUGUI>(true);

                Image[] childImgs = hwatu.GetComponentsInChildren<Image>(true); // hwatu1+ hwatu2 + skillImg 
                int synergeType = GetSynergeName(hwatu.name);

                SeotdaHwatuName[] cards = Hwatu.GetHwatuCombination((SeotdaHwatuCombination)synergeType);
                Hwatu hwatu1 = null, hwatu2 = null;
                for (int j = 0; j < SkillManager.instance.hwatuData.Length; j++)
                {
                    if (cards[0] == SkillManager.instance.hwatuData[j].hwatu.type)
                    {
                        childImgs[1].sprite = SkillManager.instance.hwatuData[j].sprite;
                        hwatu1 = SkillManager.instance.hwatuData[j].hwatu;
                    }

                    else if (cards[1] == SkillManager.instance.hwatuData[j].hwatu.type)
                    {
                        childImgs[0].sprite = SkillManager.instance.hwatuData[j].sprite;
                        hwatu2 = SkillManager.instance.hwatuData[j].hwatu;
                    }
                }

                if (hwatu1 != null && hwatu2 != null)
                {
                    SeotdaHwatuCombination result = Hwatu.GetHwatuCombination(hwatu1, hwatu2);
                    childTxts[0].text = SkillManager.instance.skillDBDictionary[result].synergyName;
                    childImgs[4].sprite = SkillManager.instance.skillSpriteDictionary[result];
                    childTxts[1].text = SkillManager.instance.GetSkillInfo(result, false);
                }
                idx++;
            }
        }
    }

    string[] synergeName;
    void SetSynergyName()
    {
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
        for (int i = 0; i < synergeName.Length; i++)
        {
            if (curSynergy == synergeName[i]) return i;
        }
        return -1;
    }

    public void JokboState(bool state)
    {   // true : open / false : close
        if (!isPossibleJokbo) return;
        
        SetUI();
        if (state)
        {
            if (Player.instance.isTutorial)
            {
                TutorialInteraction interaction = FindObjectOfType<TutorialInteraction>();
                if (interaction.curScarescrowType == ScareScrowType.Skill)
                {
                    interaction.OnJokboInBlanket();
                }
            }
            SoundManager.instance.SetEffectSound(SoundType.UI, UISfx.Jokbo);
            UIManager.instance.PushPopUI(childUI[0]);
        }
        else
            UIManager.instance.isClose = true;
    }
}
