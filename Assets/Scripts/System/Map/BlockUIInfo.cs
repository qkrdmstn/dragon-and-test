using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlockChild
{
    BG,
    Question,
    Way,
    Interaction
}

public class BlockUIInfo : MonoBehaviour, IComparable
{
    public bool isWay, isInteraction;
    public Transform[] childrens;  // bg, question, way, interaction;
    public BlockUIInfo[] nextBlock;
    int interactionNum;

    private void Awake()
    {
        InitObject();
    }

    void InitObject()
    {
        childrens = new Transform[transform.childCount];
        for(int i=0; i<transform.childCount; i++)
        {
            childrens[i] = transform.GetChild(i);

            if(childrens[i].gameObject.name == "Way") isWay = true;
            if (childrens[i].gameObject.name == "Blanket"
                || childrens[i].gameObject.name == "Shop"
                || childrens[i].gameObject.name == "Boss")
            {
                isInteraction = true;
                interactionNum = i;
            }
        }
    }

    public void SwitchChild(BlockChild childNum, bool state)
    {
        if(childNum == BlockChild.Interaction)
            childrens[interactionNum].gameObject.SetActive(state);
        else
            childrens[(int)childNum].gameObject.SetActive(state);
    }

    public int CompareTo(object obj)
    {
        Vector2 thisPos = this.transform.position;
        Vector2 comparePos = (obj as BlockUIInfo).transform.position;

        if (thisPos.y > comparePos.y)
            return -1;
        else if (thisPos.y == comparePos.y)
        {
            if (thisPos.x < comparePos.x)
                return -1;
            return 1;
        }
        else
            return 1;
    }
}
