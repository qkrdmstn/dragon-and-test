using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapControl : MonoBehaviour
{
    public GameObject myMapType;
    public GameObject gotoMapType;
    public Transform gotoPos;
    public PolygonCollider2D confineColl;

    public static string curMapName;

    private void Start()
    {
        confineColl = gotoMapType.GetComponentInChildren<PolygonCollider2D>(); ;

        if (gameObject.name.Equals("GoToRight")) gotoPos = gotoMapType.transform.Find("SpawnZone").Find("GoToLeft");
        else if (gameObject.name.Equals("GoToLeft")) gotoPos = gotoMapType.transform.Find("SpawnZone").Find("GoToRight");
        else if (gameObject.name.Equals("GoToTop")) gotoPos = gotoMapType.transform.Find("SpawnZone").Find("GoToBottom");
        else if (gameObject.name.Equals("GoToBottom")) gotoPos = gotoMapType.transform.Find("SpawnZone").Find("GoToTop");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if(curMapName != myMapType.name)
            {
                curMapName = gotoMapType.name;
                UIManager.instance.fade.ManageFade(this);   // 맵 이동에 따른 전환 효과 실행

                Debug.Log(curMapName);
                
                GameManager.instance.player.SetIdleStatePlayer();
                GameManager.instance.player.isStateChangeable = false;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && curMapName == myMapType.name)
        {
            curMapName = "";
            Debug.Log(curMapName);
        }
    }
}
