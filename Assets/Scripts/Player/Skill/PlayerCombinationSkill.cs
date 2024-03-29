using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;
using Image = UnityEngine.UI.Image;

[System.Serializable]
public struct HwatuUI
{
    public int idx;
    public Hwatu hwatu;
    public GameObject obj;

    public HwatuUI(GameObject _obj, Hwatu _hwatu, int _idx)
    {
        obj = _obj;
        hwatu = _hwatu;
        idx = _idx;
    }
}

public class PlayerCombinationSkill : MonoBehaviour
{
    [Header("Combination Skill info")]
    [SerializeField] private float combinationDuration = 6.0f;
    private float combinationTimer;
    [Space(10f)]
    [SerializeField] private float cusorFocusDuration;
    private float cusorTimer;
    [Space(10f)]
    [SerializeField] private int drawCnt;
    public bool isCombinationSkill;

    [Header("Probability info")]
    public float[] firstProb = { 0.05f, 0.05f, 0.05f, 0.05f, 0.05f, 0.05f, 0.05f, 0.05f, 0.05f, 0.05f,
                                    0.05f, 0.05f, 0.05f, 0.05f, 0.05f, 0.05f, 0.05f, 0.05f, 0.05f, 0.05f};
    private float[] secondProb;
    private float[] cumulativeProb;

    [Header("Card info")]
    [SerializeField] private HwatuUI[] selectedCards;
    private HwatuUI[] activeCardUI;
    private HwatuUI[] hwatuUI;

    private IEnumerator coroutine;

    #region Components
    private Player player;
    public SkillManager skillManager;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindObjectOfType<Player>();
        this.enabled = player.isCombatZone;

        if (player.isCombatZone)
            InitHwatuUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && player.curMP >= player.maxMP && !isCombinationSkill)
            StartCombinationSkill();

    }

    public void InitHwatuUI()
    {
        GameObject SkillUI = UIManager.instance.curUIGroup.childUI[0];
        int numOfChild = SkillUI.transform.childCount;

        hwatuUI = new HwatuUI[numOfChild];
        for (int i = 0; i < numOfChild; i++)
        {
            Transform tf = SkillUI.transform.GetChild(i);
            hwatuUI[i] = new HwatuUI(tf.gameObject, tf.GetComponent<Hwatu>(), i);
        }
    }

    private void StartCombinationSkill()
    {
        player.curMP -= player.maxMP;
        isCombinationSkill = true;
        selectedCards = new HwatuUI[2];

        coroutine = SelectCardCoroutine();
        StartCoroutine(coroutine);
    }

    IEnumerator SelectCardCoroutine()
    {
        yield return null;

        int focusIdx = 0;
        ActiveRandomCard();
        combinationTimer = combinationDuration;
        cusorTimer = cusorFocusDuration;
        while (!Input.GetKeyDown(KeyCode.E))
        {
            yield return null;
            combinationTimer -= Time.deltaTime;
            cusorTimer -= Time.deltaTime;
            SetActiveCardPos();

            if (cusorTimer < 0.0f)
            {
                cusorTimer = cusorFocusDuration;
                focusIdx++;
                focusIdx %= 3;
                ChangeCardFocus(focusIdx);
            }
            if (combinationTimer < 0.0f)
            {
                TimeOver();
            }
        }
        EndRandomCardSelect(focusIdx);
    }

    private void ActiveRandomCard()
    {
        List<int> randomIdx = GetCardRandomIdxList();
        activeCardUI = new HwatuUI[3] { hwatuUI[randomIdx[0]], hwatuUI[randomIdx[1]], hwatuUI[randomIdx[2]] };

        //Setting Card Position & Activate Card
        SetActiveCardPos();
        for (int i = 0; i < randomIdx.Count; i++)
        {
            Image img = activeCardUI[i].obj.GetComponent<Image>();
            if (i == 0)
                img.color = new Color(1, 1, 1, 1);
            else
                img.color = new Color(1, 1, 1, 0.5f);

            hwatuUI[randomIdx[i]].obj.SetActive(true);
        }
    }

    private List<int> GetCardRandomIdxList()
    {
        List<int> randomIdx = new List<int>();
        while (randomIdx.Count < 3)
        {
            int idx = GetCardRandomIdx();
            if (IndexDuplicateCheck(idx, randomIdx))
                randomIdx.Add(idx);
        }
        return randomIdx;
    }

    private bool IndexDuplicateCheck(int idx, List<int> randomIdx)
    {
        //Selected Check
        for (int i = 0; i < randomIdx.Count; i++)
        {
            if (randomIdx[i] == idx)
                return false;
        }

        for (int i = 0; i < drawCnt; i++)
        {
            if (selectedCards[i].idx == hwatuUI[idx].idx)
                return false;
        }
        return true;
    }

    private int GetCardRandomIdx()
    {
        SetCumulativeProb();

        float pivot = Random.Range(0.0f, 1.0f);
        for (int i = 0; i < cumulativeProb.Length; i++)
        {
            if (pivot < cumulativeProb[i])
            {
                return i;
            }
        }
        return -1;
    }

    private void SetCumulativeProb()
    {
        cumulativeProb = new float[20];
        float[] curProbArray = GetProbArray();
        for (int i = 0; i < curProbArray.Length; i++)
        {
            if (i == 0)
                cumulativeProb[i] = curProbArray[i];
            else
                cumulativeProb[i] += curProbArray[i] + cumulativeProb[i - 1];
        }
        cumulativeProb[19] = 1.00001f;
    }

    private float[] GetProbArray()
    {
        if (drawCnt == 0)
            return firstProb;
        else
        {
            secondProb = new float[20];
            int selectCardIdx = selectedCards[0].idx;
            float curTotalSum = 1.0f - firstProb[selectCardIdx];

            for (int i = 0; i < 20; i++)
            {
                if (i == selectCardIdx)
                    secondProb[i] = 0.0f;
                else
                {
                    float prob1 = firstProb[i] / curTotalSum;
                    secondProb[i] = firstProb[i] + (prob1 * firstProb[selectCardIdx]);
                }
            }
            return secondProb;
        }
    }

    private void ChangeCardFocus(int focusIdx)
    {
        for (int i = 0; i < 3; i++)
        {
            Image img = activeCardUI[i].obj.GetComponent<Image>();
            if (i == focusIdx)
                img.color = new Color(1, 1, 1, 1);
            else
                img.color = new Color(1, 1, 1, 0.5f);
        }
    }

    private void SetActiveCardPos()
    {
        Vector3 leftPos = this.transform.position + Vector3.up + Vector3.left;
        Vector3 middlePos = this.transform.position + Vector3.up;
        Vector3 rightPos = this.transform.position + Vector3.up + Vector3.right;

        activeCardUI[0].obj.transform.position = Camera.main.WorldToScreenPoint(leftPos);
        activeCardUI[1].obj.transform.position = Camera.main.WorldToScreenPoint(middlePos);
        activeCardUI[2].obj.transform.position = Camera.main.WorldToScreenPoint(rightPos);
    }

    private void TimeOver()
    {
        drawCnt = 0;
        isCombinationSkill = false;

        StopCoroutine(coroutine);
        for (int i = 0; i < hwatuUI.Length; i++)
        {
            hwatuUI[i].obj.SetActive(false);
        }
    }

    private void EndRandomCardSelect(int focusIdx)
    {
        selectedCards[drawCnt] = activeCardUI[focusIdx];
        drawCnt++;
        for (int i = 0; i < hwatuUI.Length; i++)
        {
            hwatuUI[i].obj.SetActive(false);
        }

        if(drawCnt == 1)
        {
            coroutine = SelectCardCoroutine();
            StartCoroutine(coroutine);
        }
        else if (drawCnt == 2)
        {
            drawCnt = 0;
            isCombinationSkill = false;
            SkillEffect(Hwatu.GetHwatuCombination(selectedCards[0].hwatu, selectedCards[1].hwatu));
        }
    }

    private void SkillEffect(HwatuCombination result)
    {
        //skillManager.SkillEffect(result);

        Debug.Log(result.ToString());
    }
}