using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScarecrowAttacked : MonoBehaviour
{
    TutorialUIGroup instance;

    private void Start()
    {
        instance = UIManager.instance.SceneUI["Tutorial"].GetComponent<TutorialUIGroup>();
    }
    //피격
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!instance.isWASD) return;   // 플레이어가 움직인 다음에 피격 파악

        if (collision.gameObject.CompareTag("Bullet"))
        {
            instance.isAttack = true;
            Destroy(collision.gameObject);
            Debug.Log("Attacked-!");
        }
    }
}
