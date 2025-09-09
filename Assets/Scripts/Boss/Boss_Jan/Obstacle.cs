using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] int durability;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("MonsterBullet") || collision.gameObject.CompareTag("Bullet"))
              OnDamaged(1);
    }

    private void OnDamaged(int damage)
    {
        durability--;
        if(durability <= 0)
        {
            Destroy(gameObject);
        }
    }
}
