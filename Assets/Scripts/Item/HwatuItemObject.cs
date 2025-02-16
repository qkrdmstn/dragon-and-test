using UnityEngine;

public class HwatuItemObject : MonoBehaviour
{
    public HwatuData hwatuData;
    private float speed = 10;

    SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        spriteRenderer.sprite = hwatuData.sprite;
    }

    void Update()
    {
        //if (curBlock.blockClear)
        //{
            //player와의 거리 계산 및 위치 이동
            float distance = Vector3.Distance(transform.position, Player.instance.transform.position);
            transform.position = Vector3.MoveTowards(transform.position, Player.instance.transform.position, speed * Time.deltaTime);

            if (distance < 0.1f)
            { //물체가 캐릭터로 이동하면서 거리가 일정 미만되면 습득
                ItemManager.instance.AddHwatuCard(hwatuData);
                SoundManager.instance.SetEffectSound(SoundType.UI, UISfx.GetHwatu);
                Destroy(gameObject);
            }
        //}
    }
}
