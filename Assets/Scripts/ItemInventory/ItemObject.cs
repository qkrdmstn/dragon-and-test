using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    [SerializeField] private ItemData itemData;

    #region Components
    private SpriteRenderer sr;
    #endregion

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            Debug.Log(itemData.itemName + "Get Item!!");
            Destroy(this.gameObject);
        }
    }
}
