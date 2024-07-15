using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class HwatuItemObject : MonoBehaviour
{
    public HwatuData hwatuData;
    public BlockInfo curBlock;
    private float speed = 10;

    Player player;
    SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        player = GameManager.instance.player;

        //어떤 block 내부에 있는지 확인
        BlockInfo[] blocks = FindObjectsOfType<BlockInfo>();
        for (int i = 0; i < blocks.Length; i++)
        {
            if (blocks[i].IsInBlock(this.transform.position))
            {
                curBlock = blocks[i];
                break;
            }
        }

        spriteRenderer.sprite = hwatuData.sprite;
    }

    // Update is called once per frame
    void Update()
    {
        //if (curBlock.blockClear)
        {
            //player와의 거리 계산 및 위치 이동
            float distance = Vector3.Distance(transform.position, player.transform.position);
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);

            if (distance < 0.1f)
            { //물체가 캐릭터로 이동하면서 거리가 일정 미만되면 습득
                SkillManager.instance.AddMaterialCardData(hwatuData);
                Destroy(gameObject);
            }
        }
    }
}
