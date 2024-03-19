using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum Month 
{
    Jan, Feb, Mar, Apr, May,
    Jun, Jul, Aug, Sep, Oct
}

public enum HwatuType
{
    JanCrane, JanPine,
    FebBird, FebPrunus,
    MarCherryLight, MarCherry,
    AprCuckoo, AprWisteria,
    MayBridge, MayIris,
    JunButterfly, JunPeony,
    JulBoar, JulLespedeza,
    AugMoon, AugGoose,
    SepSakajuki, SepChrysanthemum,
    OctDeer, OctFoliage
}

public enum HwatuCombination
{
    GTT38, GTT13, GTT18, 
    AHES74,
    JTT, MTGR94,
    TT9, TT8, TT7, TT6, TT5, TT4, TT3, TT2, TT1, TTCatch73,
    PT94,
    AL12, DS14, GPP19, JPP110, JS410, SR46,
    KK9, KK8, KK7, KK6, KK5, KK4, KK3, KK2, KK1, KK0
}

[System.Serializable]
 class HwatuCard
{
    public GameObject cardObj;
    public Month month;
    public HwatuType type;
    public PlayerSkillHwatu hwatu;
    public RectTransform rectTransform;
}

public class PlayerSkill : MonoBehaviour
{
    [Header("Skill info")]
    public int drawCnt;

    [Space(10f)]
    public float skillDuration;
    public float skillTimer;
    [Space(10f)]
    public float cardCreationDelay;
    public float cardCreationTimer;
    [Space(10f)]
    public int minActiveCardNum;
    public int curActiveCardNum;
    [Space(10f)]
    public float cardSizeX;
    public float cardSizeY;
    [Range(0.0f, 1.0f)]
    public float maxOverlapArea = 0.7f;
    [SerializeField] private HwatuCard[] selectCards;

    #region Components
    private Player player;
    public GameObject skillUI;
    public RectTransform blanketRectTransform;
    [SerializeField] HwatuCard[]hwatuCards;
    #endregion

    // Start is called before the first frame update
    void Awake()
    {
        player = GameObject.FindObjectOfType<Player>();

        InitHwatu();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q) && !skillUI.activeSelf)
        {
            if(player.isCombatZone && player.isAttackable && player.curMP >= player.maxMP)
                StartSkill();
        }
    }

    private void InitHwatu()
    {
        for (int i = 0; i < hwatuCards.Length; i++) 
        {
            hwatuCards[i].hwatu = hwatuCards[i].cardObj.GetComponent<PlayerSkillHwatu>();
            hwatuCards[i].rectTransform = hwatuCards[i].cardObj.GetComponent<RectTransform>();
        }
        cardSizeX = hwatuCards[0].rectTransform.sizeDelta.x;
        cardSizeY = hwatuCards[0].rectTransform.sizeDelta.y;
    }

    private void StartSkill()
    {
        //player setting
        //player.curMP -= player.maxMP;
        player.isStateChangeable = false;

        Time.timeScale = 0.0f;
        
        drawCnt = 0;
        skillTimer = skillDuration;
        selectCards = new HwatuCard[2];

        curActiveCardNum = 0;
        for (int i = 0; i < 20; i++)
            hwatuCards[i].cardObj.SetActive(false);
        skillUI.SetActive(true);

        StartCoroutine(DrawHwatu());
    }

    IEnumerator DrawHwatu()
    {
        yield return null;
        while (skillTimer > 0.0f && drawCnt < 2)
        {
            yield return null;
            skillTimer -= Time.unscaledDeltaTime;
            cardCreationTimer -= Time.unscaledDeltaTime;

            if (cardCreationTimer < 0.0f || curActiveCardNum < minActiveCardNum)
            {
                cardCreationTimer = cardCreationDelay;
                CardRandomGenerate();
            }
        }

        ExitSkill();
    }

    private void CardRandomGenerate()
    {
        //Creating a random number of cards
        int createNum = 0;
        if (curActiveCardNum < minActiveCardNum)
            createNum += minActiveCardNum - curActiveCardNum;
        createNum += Random.Range(0, 3);

        if (createNum + curActiveCardNum > 20)
            createNum = 20 - curActiveCardNum;

        //Determining card type random
        List<int> randomIndexes = CreatingRandomType(createNum);

        //Creating Random Pos
        List<Vector2> randomPositions = CreatingRandomPos(createNum);

        for (int i = 0; i < createNum; i++)
        {
            hwatuCards[randomIndexes[i]].rectTransform.anchoredPosition = randomPositions[i];
            hwatuCards[randomIndexes[i]].cardObj.transform.SetAsLastSibling();
            hwatuCards[randomIndexes[i]].cardObj.SetActive(true);
        }
    }

    private List<int> CreatingRandomType(int num)
    {
        List<int> indexes = new List<int>();
        while (indexes.Count < num)
        {
            int idx = Random.Range(0, 20);
            if (IndexDuplicateCheck(idx, indexes))
                indexes.Add(idx);
        }

        return indexes;
    }

    private bool IndexDuplicateCheck(int idx, List<int> indexes)
    {
        //Activated Check
        if (hwatuCards[idx].cardObj.activeSelf)
            return false;

        //Selected Check
        for(int i=0; i<drawCnt; i++)
        {
            if (selectCards[i].cardObj == hwatuCards[idx].cardObj)
                return false;
        }

        //Check the index already set to random 
        for (int i = 0; i < indexes.Count; i++)
        {
            if (indexes[i] == idx)
                return false;
        }
        return true;
    }

    private List<Vector2> CreatingRandomPos(int num)
    {
        List<Vector2> positions = new List<Vector2>();
        float minX = -blanketRectTransform.sizeDelta.x / 2.0f + cardSizeX / 2.0f;
        float maxX = minX * -1;
        float minY = -blanketRectTransform.sizeDelta.y / 2.0f + cardSizeY / 2.0f;
        float maxY = minY * -1;

        while (positions.Count < num)
        {
            float x = Random.Range(minX, maxX);
            float y = Random.Range(minY, maxY);
            Vector2 temp = new Vector2(x, y);
            if (PositionDuplicateCheck(temp, positions))
                positions.Add(temp);
        }
        return positions;
    }

    private bool PositionDuplicateCheck(Vector2 pos, List<Vector2> positions)
    {
        Vector2 minPos1 = new Vector2(pos.x - cardSizeX / 2.0f, pos.y - cardSizeY / 2.0f);
        Vector2 maxPos1 = new Vector2(pos.x + cardSizeX / 2.0f, pos.y + cardSizeY / 2.0f);

        //Check the Position of Activated Card
        for (int i = 0; i < hwatuCards.Length; i++)
        {
            if (hwatuCards[i].cardObj.activeSelf)
            {
                Vector2 activeCardPos = new Vector2(hwatuCards[i].rectTransform.anchoredPosition.x, hwatuCards[i].rectTransform.anchoredPosition.y);
                Vector2 minPos2 = new Vector2(activeCardPos.x - cardSizeX / 2.0f, activeCardPos.y - cardSizeY / 2.0f);
                Vector2 maxPos2 = new Vector2(activeCardPos.x + cardSizeX / 2.0f, activeCardPos.y + cardSizeY / 2.0f);

                if (!OverlapCondition(minPos1, maxPos1, minPos2, maxPos2))
                    return false;
            }
        }

        //Check the Position already set to random 
        for(int i=0; i<positions.Count; i++)
        {
            Vector2 otherCardPos = new Vector2(positions[i].x, positions[i].y);
            Debug.Log(otherCardPos);
            Vector2 minPos2 = new Vector2(otherCardPos.x - cardSizeX / 2.0f, otherCardPos.y - cardSizeY / 2.0f);
            Vector2 maxPos2 = new Vector2(otherCardPos.x + cardSizeX / 2.0f, otherCardPos.y + cardSizeY / 2.0f);

            if(!OverlapCondition(minPos1, maxPos1, minPos2, maxPos2))
                return false;
        }
        return true;
    }

    private bool OverlapCondition(Vector2 minPos1, Vector2 maxPos1, Vector2 minPos2, Vector2 maxPos2)
    {
        float totalArea = cardSizeX * cardSizeY;
        float overlapArea = OverlapLength(minPos1.x, maxPos1.x, minPos2.x, maxPos2.x) * OverlapLength(minPos1.y, maxPos1.y, minPos2.y, maxPos2.y);
        if (maxOverlapArea != 0.0f && overlapArea / totalArea >= maxOverlapArea)
            return false;
        else if (maxOverlapArea == 0.0f && overlapArea != 0.0f)
            return false;
        else
            return true;
    }

    private float OverlapLength(float minX1, float maxX1, float minX2, float maxX2)
    {
        float result = 0.0f;
        if (minX1 >= maxX2) return 0.0f;
        if (minX2 >= maxX1) return 0.0f;

        if (minX1 < minX2 && maxX1 > maxX2)
            result = maxX2 - minX2;
        else if (minX2 < minX1 && maxX2 > maxX1)
            result = maxX1 - maxX2;
        else if (minX1 < maxX2 && minX1 > minX2)
            result = maxX2 - minX1;
        else if (minX2 < maxX1 && minX2 > minX1)
            result = maxX1 - minX2;

        if (result < 0.0f)
            result *= -1;
        return result;
    }



    private void ExitSkill()
    {
        player.isStateChangeable = true;
        skillUI.SetActive(false);

        if(drawCnt == 2)
        {
            HwatuCombination result = CheckCombination();
            drawCnt = 0;

            UseSkill(result);
        }

        Time.timeScale = 1.0f;
    }

    private HwatuCombination CheckCombination()
    {
        int[] months = new int[] { (int)selectCards[0].month + 1, (int)selectCards[1].month + 1 };
        HwatuType[] types = new HwatuType[] { selectCards[0].type, selectCards[1].type };

        //TT Set
        if (months[0] == months[1])
        {
            switch (months[0])
            {
                case 10:
                    return HwatuCombination.JTT;
                case 9:
                    return HwatuCombination.TT9;
                case 8:
                    return HwatuCombination.TT8;
                case 7:
                    return HwatuCombination.TT7;
                case 6:
                    return HwatuCombination.TT6;
                case 5:
                    return HwatuCombination.TT5;
                case 4:
                    return HwatuCombination.TT4;
                case 3:
                    return HwatuCombination.TT3;
                case 2:
                    return HwatuCombination.TT2;
                case 1:
                    return HwatuCombination.TT1;
                default:
                    break;
            }
        }
        else if (Array.Exists(types, x => x == HwatuType.MarCherryLight) && Array.Exists(types, x => x == HwatuType.AugMoon))
            return HwatuCombination.GTT38;
        else if (Array.Exists(types, x => x == HwatuType.SepSakajuki) && Array.Exists(types, x => x == HwatuType.AprCuckoo))
            return HwatuCombination.MTGR94;
        else if (Array.Exists(types, x => x == HwatuType.JanCrane) && Array.Exists(types, x => x == HwatuType.AugMoon))
            return HwatuCombination.GTT18;
        else if (Array.Exists(types, x => x == HwatuType.JanCrane) && Array.Exists(types, x => x == HwatuType.MarCherryLight))
            return HwatuCombination.GTT13;
        else if (Array.Exists(types, x => x == HwatuType.JulBoar) && Array.Exists(types, x => x == HwatuType.AprCuckoo))
            return HwatuCombination.AHES74;
        else if (Array.Exists(months, x => x == 9) && Array.Exists(months, x => x == 4))
            return HwatuCombination.PT94;
        else if (Array.Exists(months, x => x == 7) && Array.Exists(months, x => x == 3))
            return HwatuCombination.TTCatch73;
        else if (Array.Exists(months, x => x == 1) && Array.Exists(months, x => x == 2))
            return HwatuCombination.AL12;
        else if (Array.Exists(months, x => x == 1) && Array.Exists(months, x => x == 4))
            return HwatuCombination.DS14;
        else if (Array.Exists(months, x => x == 1) && Array.Exists(months, x => x == 9))
            return HwatuCombination.GPP19;
        else if (Array.Exists(months, x => x == 1) && Array.Exists(months, x => x == 10))
            return HwatuCombination.JPP110;
        else if (Array.Exists(months, x => x == 4) && Array.Exists(months, x => x == 10))
            return HwatuCombination.JS410;
        else if (Array.Exists(months, x => x == 4) && Array.Exists(months, x => x == 6))
            return HwatuCombination.SR46;
        else  //KK Set
        {
            int sum = months[0] + months[1];
            sum %= 10;

            switch (sum)
            {
                case 9:
                    return HwatuCombination.KK9;
                case 8:
                    return HwatuCombination.KK8;
                case 7:
                    return HwatuCombination.KK7;
                case 6:
                    return HwatuCombination.KK6;
                case 5:
                    return HwatuCombination.KK5;
                case 4:
                    return HwatuCombination.KK4;
                case 3:
                    return HwatuCombination.KK3;
                case 2:
                    return HwatuCombination.KK2;
                case 1:
                    return HwatuCombination.KK1;
                case 0:
                    return HwatuCombination.KK0;
                default:
                    break;
            }
        }

        return HwatuCombination.KK0;
    }

    private void UseSkill(HwatuCombination result)
    {
        switch(result)
        {
            case HwatuCombination.GTT38:

                break;
            case HwatuCombination.MTGR94:

                break;
            case HwatuCombination.AHES74:

                break;
            case HwatuCombination.GTT13:

                break;
            case HwatuCombination.GTT18:

                break;
            case HwatuCombination.PT94:

                break;
            case HwatuCombination.JTT:

                break;
            case HwatuCombination.TTCatch73:

                break;
            case HwatuCombination.TT9:

                break;
            case HwatuCombination.TT8:

                break;
            case HwatuCombination.TT7:

                break;
            case HwatuCombination.TT6:

                break;
            case HwatuCombination.TT5:

                break;
            case HwatuCombination.TT4:

                break;
            case HwatuCombination.TT3:

                break;
            case HwatuCombination.TT2:

                break;
            case HwatuCombination.TT1:

                break;
            case HwatuCombination.AL12:

                break;
            case HwatuCombination.DS14:

                break;
            case HwatuCombination.GPP19:

                break;
            case HwatuCombination.JPP110:

                break;
            case HwatuCombination.JS410:

                break;
            case HwatuCombination.SR46:

                break;
            case HwatuCombination.KK9:

                break;
            case HwatuCombination.KK8:

                break;
            case HwatuCombination.KK7:

                break;
            case HwatuCombination.KK6:

                break;
            case HwatuCombination.KK5:

                break;
            case HwatuCombination.KK4:

                break;
            case HwatuCombination.KK3:

                break;
            case HwatuCombination.KK2:

                break;
            case HwatuCombination.KK1:

                break;
            case HwatuCombination.KK0:

                break;
        }

        player.HP += 2;
    }

    public void SelectHwatu(GameObject selectObj)
    {
        for (int i = 0; i < hwatuCards.Length; i++)
        {
            if (selectObj == hwatuCards[i].cardObj)
            {
                selectCards[drawCnt] = hwatuCards[i];
                drawCnt++;
                break;
            }
        }
    }
}
