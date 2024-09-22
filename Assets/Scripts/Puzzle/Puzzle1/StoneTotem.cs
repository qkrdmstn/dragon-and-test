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
    public bool isChanging;

    public SpriteRenderer cardImg;
    public Sprite blankImg;
    public List<Sprite> decks;

    PuzzleInteraction puzzleInteraction;

    void Start()
    {
        puzzleInteraction = Player.instance.GetComponentInChildren<PuzzleInteraction>();

        decks = new List<Sprite>(1);
        HwatuData[] datas = SkillManager.instance.hwatuData;
        Array.Sort(datas);

        for (int i=0; i<datas.Length; i++)
        {
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
        do
        {   // 퍼즐 달 제외한 랜덤한 화투패 선정
            index = UnityEngine.Random.Range(0, 10);    // 1~10월
        } while (index == (int)month);

        cardImg.sprite = decks[index];
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if(collision.CompareTag("Bullet"))
    //    {
    //        Destroy(collision.gameObject);
    //        if (!isChanging)
    //            StartCoroutine(HwatuChangeCoroutine());
    //    }
    //}

    public void OnDamaged()
    {
        if (!isChanging)
            StartCoroutine(HwatuChangeCoroutine());
    }

    IEnumerator HwatuChangeCoroutine()
    {
        isChanging = true;
        SoundManager.instance.SetEffectSound(SoundType.Puzzle, PuzzleSfx.Choose);
        cardImg.sprite = blankImg;
        yield return new WaitForSeconds(0.5f);

        index = (++index) % 10;
        cardImg.sprite = decks[index];
        if (index == (int)month)
        {
            puzzleInteraction.isClears[(int)myType] = true;
        }
        else puzzleInteraction.isClears[(int)myType] = false;

        isChanging = false;
    }
}
