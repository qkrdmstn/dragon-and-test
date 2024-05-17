using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MapControl : MonoBehaviour
{
    public GameObject myMapType;
    public GameObject gotoMapType;
    public Transform gotoPos;
    public PolygonCollider2D confineColl;

    public static int curMapNum;

    private Spawner spawner;
    public bool flag;

    private void Start()
    {
        spawner = FindObjectOfType<Spawner>();

        confineColl = gotoMapType.GetComponentInChildren<PolygonCollider2D>();

        if (gameObject.name.Equals("GoToRight")) gotoPos = gotoMapType.transform.Find("SpawnZone").Find("GoToLeft");
        else if (gameObject.name.Equals("GoToLeft")) gotoPos = gotoMapType.transform.Find("SpawnZone").Find("GoToRight");
        else if (gameObject.name.Equals("GoToTop")) gotoPos = gotoMapType.transform.Find("SpawnZone").Find("GoToBottom");
        else if (gameObject.name.Equals("GoToBottom")) gotoPos = gotoMapType.transform.Find("SpawnZone").Find("GoToTop");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        BlockInfo myBlockInfo = myMapType.GetComponent<BlockInfo>();
        if (collision.CompareTag("Player") && myBlockInfo.blockClear && !flag)
        {
            int myMapNum = myBlockInfo.blockNumber;
            if (curMapNum != myMapNum)
            {
                flag = true;
                curMapNum = gotoMapType.GetComponent<BlockInfo>().blockNumber;
                UIManager.instance.fade.ManageFade(this, spawner, curMapNum);   // 맵 이동에 따른 전환 효과 실행
                Debug.Log(curMapNum);

                GameManager.instance.player.SetIdleStatePlayer();
                GameManager.instance.player.isStateChangeable = false;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        int myMapNum = myMapType.GetComponent<BlockInfo>().blockNumber;
        if (collision.CompareTag("Player") && curMapNum == myMapNum)
        {

            curMapNum = -1;
            Debug.Log(curMapNum);
        }
    }
}
