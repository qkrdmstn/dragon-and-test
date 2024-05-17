using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StoneTotem : MonoBehaviour
{
    public bool isClear;
    public int index;
    public HwatuMonth monthAnswer;
    public HwatuType typeAnswer;
    public bool isChanging;
    [SerializeField] private TotemHwatu[] hwatuObjs;
    [SerializeField] private GameObject blankObj;
    [SerializeField] private Puzzle1Manager puzzleManager;

    // Start is called before the first frame update
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


        if (hwatuObjs[index].hwatuMonth == monthAnswer && hwatuObjs[index].hwatuType == typeAnswer)
        {
            isClear = true;
            puzzleManager.ClearCheck();
        }
        isChanging = false;
    }
}
