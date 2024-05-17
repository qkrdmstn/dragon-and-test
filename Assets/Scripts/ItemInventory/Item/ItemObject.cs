using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    public ItemData itemData;

    public void SetItemData()
    {
        GetComponent<SpriteRenderer>().sprite = itemData.icon;
        gameObject.name = "Item Obj - " + itemData.itemName;

        GetComponent<BoxCollider2D>().size = new Vector2(transform.localScale.x, 2);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if(collision.CompareTag("Player"))
        //{
        //    Debug.Log(itemData.itemName + "Get Item!!");
        //    InventoryData.instance.AddItem(itemData);
        //    Destroy(this.gameObject);
        //}
    }
}
