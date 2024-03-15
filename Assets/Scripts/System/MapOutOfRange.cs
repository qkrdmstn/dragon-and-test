using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapOutOfRange : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            UIManager.instance.fade.ManageFade(true, gameObject.name);
        }
    }
}
