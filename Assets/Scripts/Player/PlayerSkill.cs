using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

#region Enum
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
#endregion

#region Class
[System.Serializable]
public class HwatuCard
{
    public GameObject cardObj;
    public Month month;
    public HwatuType type;
    public PlayerSkillHwatu hwatu;
    public RectTransform rectTransform;
}

class Sero
{
    public int x1;
    public int y1, y2;
    public int chk;

    public Sero(int _x1, int _y1, int _y2, int _chk)
    {
        x1 = _x1;
        y1 = _y1;
        y2 = _y2;
        chk = _chk;
    }
}
#endregion


public class PlayerSkill : MonoBehaviour
{
    [Header("Card info")]
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
    private float cardSizeX;
    private float cardSizeY;
    [Range(0.0f, 1.0f)]
    [SerializeField] private float maxOverlapArea = 0.7f;

    public HwatuCard[] selectCards;
    private Vector2 selectCardWorldPos;

    [Header("Damage info")]
    [SerializeField] private float damageRadius;
    [SerializeField] private int skillDamage;
    [SerializeField] private LayerMask monsterLayer;

    #region Components
    private Player player;
    [Space(10f)]
    public GameObject skillUI;
    public RectTransform blanketRectTransform;
    [SerializeField] HwatuCard[]hwatuCards;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindObjectOfType<Player>();
        
        if(player.isCombatZone)
            InitHwatu();
    }

    // Update is called once per frame
    void Update()
    {
        if(player.isCombatZone && Input.GetKeyDown(KeyCode.Q) && !skillUI.activeSelf)
        {
            if(player.isCombatZone && player.isAttackable && player.curMP >= player.maxMP)
                StartSkill();
        }
    }

    private void InitHwatu()
    {
        PlayerSkillHwatu[] cards = skillUI.GetComponentsInChildren<PlayerSkillHwatu>();
        for (int i = 0; i < hwatuCards.Length; i++) 
        {
            cards[i].CardInit();
            hwatuCards[i] = cards[i].card;
        }
        cardSizeX = hwatuCards[0].rectTransform.sizeDelta.x;
        cardSizeY = hwatuCards[0].rectTransform.sizeDelta.y;
    }

    private void StartSkill()
    {
        //player setting
        player.curMP -= player.maxMP;
        player.isStateChangeable = false;
        player.isAttackable = false;

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

        List<Sero> seroList = new List<Sero>(); //List for Card Position Overlap Test
        int[] heightArray = new int[(int)(cardSizeY * 10) + 1];

        //Check the Position of Activated Card
        for (int i = 0; i < hwatuCards.Length; i++)
        {
            if (hwatuCards[i].cardObj.activeSelf)
            {
                Vector2 activeCardPos = new Vector2(hwatuCards[i].rectTransform.anchoredPosition.x, hwatuCards[i].rectTransform.anchoredPosition.y);
                Vector2 minPos2 = new Vector2(activeCardPos.x - cardSizeX / 2.0f, activeCardPos.y - cardSizeY / 2.0f);
                Vector2 maxPos2 = new Vector2(activeCardPos.x + cardSizeX / 2.0f, activeCardPos.y + cardSizeY / 2.0f);
                AddOverlapAreas(minPos1, maxPos1, minPos2, maxPos2, seroList);
                //if (!OverlapCondition(minPos1, maxPos1, minPos2, maxPos2))
                //    return false;
            }
        }

        //Check the Position already set to random 
        for(int i=0; i<positions.Count; i++)
        {
            Vector2 otherCardPos = new Vector2(positions[i].x, positions[i].y);
            Vector2 minPos2 = new Vector2(otherCardPos.x - cardSizeX / 2.0f, otherCardPos.y - cardSizeY / 2.0f);
            Vector2 maxPos2 = new Vector2(otherCardPos.x + cardSizeX / 2.0f, otherCardPos.y + cardSizeY / 2.0f);
            AddOverlapAreas(minPos1, maxPos1, minPos2, maxPos2, seroList);
            //if(!OverlapCondition(minPos1, maxPos1, minPos2, maxPos2))
            //    return false;
        }
        //return true;

        seroList.Sort(seroCompare);

        float overlapAreas = 0.0f;
        int lx = 0;
        for (int i = 0; i < seroList.Count; i++) 
        {
            Sero s = seroList[i];
            int cnt = 0;
            for (int j = 0; j <= (int)(cardSizeY * 10); j++)
            {
                if (heightArray[j] > 0) cnt++;
            }

            overlapAreas += cnt * (s.x1 - lx);
            for (int j = s.y1 + 1; j <= s.y2; j++)
            {
                if (s.chk == 1) heightArray[j]++;
                else heightArray[j]--;
            }
            lx = s.x1;
        }
        overlapAreas /= 100.0f;

        float totalArea = cardSizeX * cardSizeY;
        if (overlapAreas / totalArea > maxOverlapArea)
            return false;
        else
        {
            //Debug.Log("position: " + pos + " Overlap Ratio: " + overlapAreas / totalArea);
            return true;
        }
    }

    private void AddOverlapAreas(Vector2 minPos1, Vector2 maxPos1, Vector2 minPos2, Vector2 maxPos2, List<Sero> list)
    {
        //Calculate Overlap Area
        float leftX = Mathf.Max(minPos1.x, minPos2.x);
        float rightX = Mathf.Min(maxPos1.x, maxPos2.x);
        float bottomY = Mathf.Max(minPos1.y, minPos2.y);
        float topY = Mathf.Min(maxPos1.y, maxPos2.y);

        float width = Mathf.Max(0, rightX - leftX);
        float height = Mathf.Max(0, topY - bottomY);

        if (width != 0 && height != 0)
        {
            float x1 = leftX * 10; float x2 = rightX * 10;
            float y1 = (bottomY - minPos1.y) * 10; float y2 = (topY - minPos1.y) * 10;
            list.Add(new Sero((int)x1, (int)y1, (int)y2, 1));
            list.Add(new Sero((int)x2, (int)y1, (int)y2, -1));
        }
    }

    private int seroCompare(Sero a, Sero b)
    {
        if (a.x1 == b.x1)
            return a.chk < b.chk ? -1 : 1;
        else 
            return a.x1 < b.x1 ? -1 : 1;
    }

    private bool OverlapCondition(Vector2 minPos1, Vector2 maxPos1, Vector2 minPos2, Vector2 maxPos2)
    {
        //Calculate Overlap Area
        float leftX = Mathf.Max(minPos1.x, minPos2.x);
        float rightX = Mathf.Min(maxPos1.x, maxPos2.x);
        float bottomY = Mathf.Max(minPos1.y, minPos2.y);
        float topY = Mathf.Min(maxPos1.y, maxPos2.y);

        float width = Mathf.Max(0, rightX - leftX);
        float height = Mathf.Max(0, topY - bottomY);
        float overlapArea = width * height;

        //Check Overlap condition 
        float totalArea = cardSizeX * cardSizeY;
        float overlapRatio = overlapArea / totalArea;
        if (overlapRatio > maxOverlapArea)
            return false;
        else
            return true;
    }

    private void ExitSkill()
    {
        player.isAttackable = true;
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

        player.moveSpeed *= 1.2f;
        player.dashSpeed *= 1.2f;
    }

    public void SelectHwatu(GameObject selectObj)
    {
        for (int i = 0; i < hwatuCards.Length; i++)
        {
            if (selectObj == hwatuCards[i].cardObj)
            {
                selectCards[drawCnt] = hwatuCards[i];
                AreaDamage(drawCnt);
                drawCnt++;
                break;
            }
        }
    }

    private void AreaDamage(int draCnt)
    {
        HwatuCard card = selectCards[draCnt];
        float yScreenHalfSize = Screen.height / 2;
        float xScreenHalfSize = Screen.width / 2;
        Vector2 cardScreenPos = card.rectTransform.anchoredPosition + new Vector2(xScreenHalfSize, yScreenHalfSize);
        selectCardWorldPos = Camera.main.ScreenToWorldPoint(cardScreenPos);

        Collider2D[] inRangeMonsters = Physics2D.OverlapCircleAll(selectCardWorldPos, damageRadius, monsterLayer);
        for(int i=0; i<inRangeMonsters.Length; i++)
        {
            Monster monster = inRangeMonsters[i].GetComponent<Monster>();
            MonsterNear monsterNear = inRangeMonsters[i].GetComponent<MonsterNear>();

            if(monster != null)
                monster.OnDamaged(skillDamage);
            else if(monsterNear != null)
                monsterNear.OnDamaged(skillDamage);

        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(selectCardWorldPos, damageRadius);
    }
}
