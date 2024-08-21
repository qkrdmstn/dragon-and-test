using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneTotem : MonoBehaviour
{
    public enum TotemType
    {
        Main, Sub
    }
    public TotemType myType;
    public HwatuMonth month;
    public SeotdaHwatuName myCard;

    public int index = -1;
    public int answerIdx = -1;
    public bool isChanging;

    public SpriteRenderer cardImg;
    public Sprite blankImg;
    public List<Sprite> decks;

    Puzzle1Manager puzzleManager;

    void Start()
    {
        puzzleManager = FindObjectOfType<Puzzle1Manager>();

        HwatuData[] datas = SkillManager.instance.hwatuData;
        for(int i=0; i<datas.Length; i++)
        {
            if(myCard == datas[i].hwatu.type) answerIdx = i;

            switch (myType)
            {
                case TotemType.Main:
                    if (datas[i].hwatu.isMain)
                        decks.Add(datas[i].sprite);
                    break;
                case TotemType.Sub:
                    if (!datas[i].hwatu.isMain)
                        decks.Add(datas[i].sprite);
                    break;
            }
        }
        Array.Sort(datas);
        do
        {   // 퍼즐 달 제외한 랜덤한 화투패 선정
            index = UnityEngine.Random.Range(0, 10);
        } while (index == (int)month);

        cardImg.sprite = decks[index];
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Bullet"))
        {
            collision.GetComponent<Bullet>().InActiveBullet();

            if (!isChanging)
                StartCoroutine(HwatuChangeCoroutine());
        }
    }

    IEnumerator HwatuChangeCoroutine()
    {
        isChanging = true;

        cardImg.sprite = blankImg;
        yield return new WaitForSeconds(0.5f);

        index = (++index) % 10;
        cardImg.sprite = decks[index];
        if (index == answerIdx)
        {
            puzzleManager.isClear[(int)myType] = true;
        }
        else puzzleManager.isClear[(int)myType] = false;

        isChanging = false;
    }
}
