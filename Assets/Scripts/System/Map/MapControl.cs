using System;
using UnityEngine;

public class MapControl : MonoBehaviour
{
    public GameObject myMapType;
    public GameObject gotoMapType;
    public Transform gotoPos;
    public PolygonCollider2D confineColl;

    MapIndicator mapIndicator;
    Collider2D coll;
    public static int curMapNum;

    private Spawner spawner;
    public bool flag;

    #region FindPortal
    enum Dir  {  Top, Bottom, Right, Left }
    int layerMask;
    Vector2Int[] dirs;
    string myGoTo;
    Vector2Int goToDir;
    #endregion

    private void Awake()
    {
        coll = GetComponent<Collider2D>();

        dirs = new Vector2Int[4];
        dirs[0] = new Vector2Int(0, 1);
        dirs[1] = new Vector2Int(0, -1);
        dirs[2] = new Vector2Int(1, 0);
        dirs[3] = new Vector2Int(-1, 0);

        layerMask = 1 << LayerMask.NameToLayer("Portal");
        myGoTo = gameObject.name.Substring(4);
        goToDir = dirs[(int)Enum.Parse<Dir>(myGoTo)];

        myMapType = transform.parent.parent.gameObject;

        spawner = FindObjectOfType<Spawner>();
        mapIndicator = FindObjectOfType<MapIndicator>();

        FindGoToPos();
    }

    void FindGoToPos()
    {
        RaycastHit2D[] hit = new RaycastHit2D[1];
        if(coll.Raycast(goToDir, hit, 10f, layerMask) > 0)
        {
            gotoPos = hit[0].transform;
            gotoMapType = gotoPos.parent.parent.gameObject;
            confineColl = gotoMapType.GetComponentInChildren<PolygonCollider2D>();
        }
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

                UIManager.instance.SetFadeObjState(true);
                UIManager.instance.fade.ManageFade(this, spawner, curMapNum);   // 맵 이동에 따른 전환 효과 실행
                mapIndicator.MoveBlockPlayer(curMapNum);

                Player.instance.SetIdleStatePlayer();
                Player.instance.isStateChangeable = false;
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
