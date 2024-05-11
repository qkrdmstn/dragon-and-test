using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkill : MonoBehaviour
{
    [Header("Skill info")]
    [Space(10f)]
    private int impactLayerMask;
    float _impactRadius;
    #region Components
    private Player player;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Player>();
        impactLayerMask = LayerMask.GetMask("Monster", "MonsterBullet");
    }

    public void UseSkill(SeotdaHwatuName name, int damage, float range, float force)
    {
        Debug.Log(name + " " + damage + " " + range + " " + force);
        if (name == SeotdaHwatuName.SepSakajuki)
            BlankBullet(damage, range, force);
        else if (name == SeotdaHwatuName.JunButterfly)
            BlankBullet(damage, range, force);
        else
            BlankBullet(damage, range, force);
    }

    private void BlankBullet(int damage, float impactRadius, float impactForce)
    {
        _impactRadius = impactRadius;

        Debug.Log("BlankBullet");
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
                monster.OnDamaged(damage);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, _impactRadius);
    }

}
