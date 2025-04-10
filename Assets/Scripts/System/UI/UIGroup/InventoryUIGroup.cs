using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUIGroup : UIGroup
{
    public GameObject[] inventoryPages; 

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I) && !childUI[0].activeSelf && !Player.instance.isInteraction)
        {
            OpenInventory();
        }
        else if ((Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.Escape)) && childUI[0].activeSelf) 
        {
            CloseInventory();
        }
    }

    private void OpenInventory()
    {
        UIManager.instance.PushPopUI(childUI[0]);
        Time.timeScale = 0.0f;
    }

    public void CloseInventory()
    {
        Time.timeScale = 1.0f;
        ChangePage(0);
        UIManager.instance.PushPopUI();
    }

    public void ChangePage(int idx)
    {
        for(int i=0; i< inventoryPages.Length; i++)
        {
            if (i == idx)
                inventoryPages[i].SetActive(true);
            else
                inventoryPages[i].SetActive(false);
        }
    }
}
