using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHit : MonoBehaviour
{
    public Player player;
    private void Awake()
    {
        player = FindObjectOfType<Player>();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("MonsterBullet") || collision.gameObject.CompareTag("Monster"))
        {
            Boss boss = collision.GetComponent<Boss>();
            if(boss == null || !boss.isDead)
                player.OnDamamged(1);
        }

        if(collision.CompareTag("Cliff"))
        {
            player.ChangeFallState();
        }
    }
}
