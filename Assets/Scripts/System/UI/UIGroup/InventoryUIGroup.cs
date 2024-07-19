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

    public override void ToggleUI(GameObject _ui)
    {
        base.ToggleUI(_ui);
    }

    private void OpenInventory()
    {
        Time.timeScale = 0.0f;
        Player.instance.isStateChangeable = false;
        Player.instance.isAttackable = false;
        childUI[0].SetActive(true);

    }

    public void CloseInventory()
    {
        Time.timeScale = 1.0f;
        Player.instance.isStateChangeable = true;
        Player.instance.isAttackable = true;
        childUI[0].SetActive(false);

        ChangePage(0);
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
