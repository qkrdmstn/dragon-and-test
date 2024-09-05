using System.Collections;
using UnityEngine;

public class BulletGenerator : MonoBehaviour
{
    [SerializeField] GameObject bullets;
    [SerializeField] float bulletSpeed;
    [SerializeField] float spawnSpeed;
    [SerializeField] int spawnCnt;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!TutorialInteraction.generateBullet) return;
        if (collision.CompareTag("Player"))
        {
            TutorialInteraction.generateBullet = false;
            GetComponentInParent<TutorialInteraction>().isActiveDone = true;
            StartCoroutine(InstantiateBullets());
        }
    }

    IEnumerator InstantiateBullets()
    {
        for(int i=0; i< spawnCnt; i++)
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
