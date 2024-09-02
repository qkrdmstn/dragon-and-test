using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyItemObject : MonoBehaviour
{
    public int amount = 1;
    private float speed = 10;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(transform.position, Player.instance.transform.position);
        transform.position = Vector3.MoveTowards(transform.position, Player.instance.transform.position, speed * Time.deltaTime);

        if (distance < 0.1f)
        { //물체가 캐릭터로 이동하면서 거리가 일정 미만되면 습득
            Player.instance.money += amount;
            Destroy(gameObject);
        }
    }
}
