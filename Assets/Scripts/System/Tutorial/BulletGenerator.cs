using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletGenerator : MonoBehaviour
{
    [SerializeField] GameObject bullets;
    [SerializeField] float bulletSpeed;
    [SerializeField] float spawnSpeed;
    bool isStart = false;

    float posX, collX;

    private void Start()
    {
        posX = transform.position.x;
        collX = posX + GetComponent<BoxCollider2D>().offset.x;
    }

    private void Update()
    {
        if (isStart)
        {
            if(Player.instance.transform.position.x > collX)
            {
                GetComponent<BoxCollider2D>().isTrigger = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!Tutorial.generateBullet) return;
        if (!isStart && collision.CompareTag("Player"))
        {
            GetComponentInParent<Tutorial>().isActiveDone = true;
            isStart = true;
            StartCoroutine(InstantiateBullets());
        }
    }

    IEnumerator InstantiateBullets()
    {
        for(int i=0; i<4; i++)
        {
            GameObject tmpGO = Instantiate(bullets, transform);
            foreach(Rigidbody2D rigid in tmpGO.GetComponentsInChildren<Rigidbody2D>())
            {
                rigid.velocity = Vector2.left * bulletSpeed;
            }
            
            yield return new WaitForSeconds(spawnSpeed);
        }
    }
}
