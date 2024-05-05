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
    private int impactLayerMask;

    #region Components
    private Player player;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Player>();
        impactLayerMask = LayerMask.GetMask("Monster", "MonsterBullet");
    }


    private void BlankBullet()
    {

        Collider2D[] inRangeTarget = Physics2D.OverlapCircleAll(this.transform.position, impactRadius, impactLayerMask);
        for (int i = 0; i < inRangeTarget.Length; i++)
        {
            GameObject target = inRangeTarget[i].gameObject;
            if (target.CompareTag("MonsterBullet"))
            {
                //Todo. Change from Monster Bullet Pool to inactive
                Destroy(inRangeTarget[i].gameObject);

            }
            else if (target.CompareTag("Monster"))
            {
                MonsterBase monster = target.GetComponent<MonsterBase>();
                Vector2 impactDir = target.transform.position - this.transform.position;
                impactDir.Normalize();

                monster.Knockback(impactDir, impactForce);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, impactRadius);
    }

    public void UseSkill(SeotdaHwatuName name, int damage, float range, float force)
    {
        Debug.Log(name + " " + damage + " " + range + " " + force);
    }
}
