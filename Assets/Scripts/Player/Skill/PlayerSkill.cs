using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkill : MonoBehaviour
{
    [Header("Blank Bullet info")]
    public float blankBulletDelay;
    public float blankBulletTimer;
    [Space(10f)]
    public float impactRadius;
    public float impactForce;
    public int impactLayerMask;

    [Header("Card Select Skill info")]
    public float cardSelectCoolTime;
    public float cardSelectTimer;
    [Space(10f)]
    public int drawCnt;
    public Hwatu[] selectCards;

    #region Components
    private Player player;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindObjectOfType<Player>();
        impactLayerMask = LayerMask.GetMask("Monster", "MonsterBullet");
    }

    // Update is called once per frame
    void Update()
    {
        blankBulletTimer -= Time.deltaTime;
        cardSelectTimer -= Time.deltaTime;

        if(Input.GetKeyDown(KeyCode.Q) && blankBulletTimer < 0.0f && player.blankBulletNum > 0)
        {
            BlankBullet();
        }

    }

    private void BlankBullet()
    {
        blankBulletTimer = blankBulletDelay;
        player.blankBulletNum--;

        Collider2D[] inRangeTarget = Physics2D.OverlapCircleAll(this.transform.position, impactRadius, impactLayerMask);
        for(int i=0; i< inRangeTarget.Length; i++)
        {
            GameObject target = inRangeTarget[i].gameObject;
            if (target.CompareTag("MonsterBullet"))
            {
                //Todo. Change from Monster Bullet Pool to inactive

            }
            else if(target.CompareTag("Monster"))
            {
                Rigidbody2D targetRigid = inRangeTarget[i].GetComponent<Rigidbody2D>();
                //Vector2 impactDir = target.transform.position - this.transform.position;
                //impactDir.Normalize();
                //targetRigid.AddForce(impactForce * impactDir);

                //Todo. Add knockback to monster state and call
            }

            Destroy(inRangeTarget[i].gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, impactRadius);
    }

}
