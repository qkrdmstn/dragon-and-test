using System.Collections;
using UnityEngine;

public class StoneTotem : MonoBehaviour
{
    public enum TotemType
    {
        Main, Sub
    }
    public TotemType myType;

    public SeotdaHwatuName myCard;
    public int index;
    public bool isChanging;
    [SerializeField] private TotemHwatu[] hwatuObjs;
    [SerializeField] private GameObject blankObj;
    [SerializeField] private Puzzle1Manager puzzleManager;

    /*
     * 필요한 기능
     * 1. 불렛에 맞아서 확인하는 기능
     * 2. 성공에 대한 보스 맵 이동 활성화
     * 3. 실패에 대한 플레이어 패널티 기능
     * 4. 스프라이트가 바뀌는 형식으로 코드 변경
     * 
     * 궁금한 것
     * Q1. 실패에 대한 판결이 있다면 -> 뭔가 확정버튼이 있어야하지 않나 -> "4개 다 건드리면" 이라는 기준은 너무 애매한거같다 -> 플레이어가 하나만 고치고 싶은 경우의 수도 있을 것.
     * Q2. 퍼즐 필드에서는 어떤 UI가 활성화 되는가
     */

    void Start()
    {
        puzzleManager = FindObjectOfType<Puzzle1Manager>();

        hwatuObjs = new TotemHwatu[transform.childCount - 1];
        for (int i=0; i < transform.childCount; i++)
        {
            if (i < transform.childCount - 1)
                hwatuObjs[i] = transform.GetChild(i).GetComponent<TotemHwatu>();
            else
                blankObj = transform.GetChild(i).gameObject;
        }

        index = Random.Range(0, 11);
        hwatuObjs[index].gameObject.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Bullet"))
        {
            collision.GetComponent<Bullet>().InActiveBullet();

            if(!isChanging)
                StartCoroutine(HwatuChangeCoroutine());
            
        }
    }

    private IEnumerator HwatuChangeCoroutine()
    {
        isChanging = true;
        hwatuObjs[index].gameObject.SetActive(false);
        blankObj.gameObject.SetActive(true);
        index++;
        index %= 12;

        yield return new WaitForSeconds(0.5f);

        blankObj.gameObject.SetActive(false);
        hwatuObjs[index].gameObject.SetActive(true);

        puzzleManager.ClearCheck();

        isChanging = false;
    }
}
