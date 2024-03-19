using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chase : MonoBehaviour
{
    public GameObject player;
    public float width = 0.5f;
    public float rayDist = 1.5f;

    //레이캐스트 결과를 저장하는 배열
    private bool[] forwardCoef = new bool[12];
    //내적값을 저장하는 배열
    private float[] dotProductValues = new float[12];
    private Vector2 targetVector;
    private float angleInterval = 360f/12f;

    private Vector2[] directions = new Vector2[12];
    public Vector2 tempDir = Vector2.zero;
    private int tempIndex = 0;
    private int[] timer = new int[12];
    private int cooldown = 200;
    private int layerMask;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player");
        layerMask = LayerMask.GetMask("Ground", "Monster");
        for (int i = 0; i < 12; i++)
        {
            float angle = i * angleInterval;
            Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            directions[i] = direction;

            forwardCoef[i] = true;
        }
    }

    void Update()
    {
        targetVector = player.transform.position - transform.position;
        
        tempIndex = calDotProduct();
        tempDir = directions[tempIndex];

        for (int i = 0; i < 12; i++)
        {
            if(timer[i]==0) forwardCoef[i] = true;
            if(dotProductValues[i]>0) Debug.DrawRay(transform.position,  directions[i] * dotProductValues[i] * 2);
        }

        timerMove();
    }

    int calDotProduct()
    {
        Vector2 rightDir = new Vector2(tempDir.y, -tempDir.x);
        Vector2 leftDir = new Vector2(-tempDir.y, tempDir.x);

        rightDir += tempDir * width;
        leftDir += tempDir * width;
        
        RaycastHit2D hitCenter = Physics2D.Raycast(transform.position+(Vector3)(tempDir * width), tempDir, rayDist, layerMask);
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position+(Vector3)(rightDir*width), tempDir, rayDist*2, layerMask);
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position+(Vector3)(leftDir*width), tempDir, rayDist*2, layerMask);
        
        Debug.DrawRay(transform.position+(Vector3)(tempDir * width), tempDir*rayDist, Color.red);
        Debug.DrawRay(transform.position+(Vector3)(rightDir*width), tempDir*rayDist*2, Color.red);
        Debug.DrawRay(transform.position+(Vector3)(leftDir*width), tempDir*rayDist*2, Color.red);

        if (hitCenter.collider != null)
        {
            forwardCoef[(tempIndex+10)%12] = false;
            forwardCoef[(tempIndex+11)%12] = false;
            forwardCoef[tempIndex] = false;    
            forwardCoef[(tempIndex+1)%12] = false;
            forwardCoef[(tempIndex+2)%12] = false;

            timer[(tempIndex+10)%12] = cooldown;
            timer[(tempIndex+11)%12] = cooldown;
            timer[tempIndex] = cooldown;   
            timer[(tempIndex+1)%12] = cooldown;
            timer[(tempIndex+2)%12] = cooldown;
        }

        if (hitRight.collider != null)
        {
            forwardCoef[(tempIndex+8)%12] = false;
            forwardCoef[(tempIndex+9)%12] = false;
            forwardCoef[(tempIndex+10)%12] = false;    
            forwardCoef[(tempIndex+11)%12] = false;
            forwardCoef[tempIndex] = false;

            timer[(tempIndex+8)%12] = cooldown;
            timer[(tempIndex+9)%12] = cooldown;
            timer[(tempIndex+10)%12] = cooldown;   
            timer[(tempIndex+11)%12] = cooldown;
            timer[tempIndex] = cooldown;
        }

        if (hitLeft.collider != null)
        {
            forwardCoef[tempIndex] = false;
            forwardCoef[(tempIndex+1)%12] = false;
            forwardCoef[(tempIndex+2)%12] = false;    
            forwardCoef[(tempIndex+3)%12] = false;
            forwardCoef[(tempIndex+4)%12] = false;

            timer[tempIndex] = cooldown;
            timer[(tempIndex+1)%12] = cooldown;
            timer[(tempIndex+2)%12] = cooldown;   
            timer[(tempIndex+3)%12] = cooldown;
            timer[(tempIndex+4)%12] = cooldown;
        }

        int maxIndex = 0;
        float maxVal = -100;
        for (int i = 0; i < 12; i++)
        {
            if (forwardCoef[i]==false)
            {
                dotProductValues[i] = -100;
                continue;
            }
            else
            {
                dotProductValues[i] = Vector2.Dot(directions[i].normalized, targetVector.normalized);

            }

            if (dotProductValues[i] > maxVal)
            {  
                maxVal = dotProductValues[i];
                maxIndex = i;
            }
        }

        return maxIndex;
    }

    void timerMove()
    {
        for (int i = 0; i < 12; i++)
        {
            if(timer[i]>0) timer[i] -= 1;
        }
    }

}