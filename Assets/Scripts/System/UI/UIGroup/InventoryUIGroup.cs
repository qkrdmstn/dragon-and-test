using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUIGroup : UIGroup
{
    public GameObject[] inventoryPages; 
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I) && !childUI[0].activeSelf && !GameManager.instance.player.isInteraction)
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
        GameManager.instance.player.isStateChangeable = false;
        GameManager.instance.player.isAttackable = false;
        childUI[0].SetActive(true);

    }

    public void CloseInventory()
    {
        Time.timeScale = 1.0f;
        GameManager.instance.player.isStateChangeable = true;
        GameManager.instance.player.isAttackable = true;
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
