using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chase : MonoBehaviour
{
    public GameObject player;

    //내적값을 저장하는 배열
    private float[] dotProductValues = new float[12];
    private Vector2 targetVector;
    private float angleInterval = 360f/12f;

    private Vector2[] directions = new Vector2[12];
    private Vector2 tempDir;

    void OnDrawGizmosSelected()
    {
        for (int i = 0; i < 12; i++)
        {
            float angle = i * angleInterval;
            Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

            directions[i] = direction;
            Gizmos.DrawRay(transform.position, direction * 2f);
            //Gizmos.DrawLine(transform.position, player.transform.position);
        }

        if (tempDir != null)
        {
            Gizmos.DrawRay(transform.position, tempDir * 10f);
        }
    }

    void Update()
    {
        targetVector = player.transform.position - transform.position;
        tempDir = directions[calDotProduct()];
    }

    int calDotProduct()
    {
        int maxIndex = 0;
        float maxVal = -1;
        for (int i = 0; i < 12; i++)
        {
            dotProductValues[i] = Vector2.Dot(directions[i], targetVector);
            if (dotProductValues[i] > maxVal)
            {
                maxVal = dotProductValues[i];
                maxIndex = i;
            }
        }

        return maxIndex;
    }


}