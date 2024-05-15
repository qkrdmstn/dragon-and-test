using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkill : MonoBehaviour
{
    [Header("Skill info")]
    [Space(10f)]
    private int impactLayerMask;
    float _impactRadius;
    public float testDashRange;
    #region Components
    private Player player;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Player>();
        impactLayerMask = LayerMask.GetMask("Monster", "MonsterBullet");
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.G))
        {
            DashSkill(testDashRange);
        }
    }
    public void UseSkill(SeotdaHwatuName name, int damage, float range, float force)
    {
        Debug.Log(name + " " + damage + " " + range + " " + force);
        switch(name)
        {
            //Main Card
            case SeotdaHwatuName.JanCrane:
                break;
            case SeotdaHwatuName.FebBird:
                break;
            case SeotdaHwatuName.MarCherryLight:
                break;
            case SeotdaHwatuName.AprCuckoo:
                break;
            case SeotdaHwatuName.MayBridge:
                break;
            case SeotdaHwatuName.JunButterfly:
                BlankBullet(damage, range, force);
                break;
            case SeotdaHwatuName.JulBoar:
                break;
            case SeotdaHwatuName.AugMoon:
                break;
            case SeotdaHwatuName.SepSakajuki:
                BlankBullet(damage, range, force);
                break;
            case SeotdaHwatuName.OctDeer:
                break;
            //Sub Card
            case SeotdaHwatuName.JanPine:
                DashSkill(range);
                break;
            case SeotdaHwatuName.FebPrunus:
                DashSkill(range);
                break;
            case SeotdaHwatuName.MarCherry:
                DashSkill(range);
                break;
            case SeotdaHwatuName.AprWisteria:
                DashSkill(range);
                break;
            case SeotdaHwatuName.MayIris:
                DashSkill(range);
                break;
            case SeotdaHwatuName.JunPeony:
                DashSkill(range);
                break;
            case SeotdaHwatuName.JulLespedeza:
                DashSkill(range);
                break;
            case SeotdaHwatuName.AugGoose:
                DashSkill(range);
                break;
            case SeotdaHwatuName.SepChrysanthemum:
                DashSkill(range);
                break;
            case SeotdaHwatuName.OctFoliage:
                DashSkill(range);
                break;
        }

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

    private void DashSkill(float dist)
    {
        StartCoroutine(DashSkillCoroutine(dist));
    }

    IEnumerator DashSkillCoroutine(float dist)
    {
        //Change Layer & Change Color
        gameObject.layer = 14;
        player.isStateChangeable = false;

        float curDist = 0.0f;
        float vel = 40;

        //Initial Direction Setting
        Vector2 dir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - this.transform.position;
        dir.Normalize();

        while (dist >= curDist)
        {
            float dv = Time.deltaTime * vel;
            Vector2 ds = Time.deltaTime * vel * dir;
            gameObject.transform.position += new Vector3(ds.x, ds.y, 0);
            curDist += dv;
            yield return new WaitForFixedUpdate();
        }

        player.isStateChangeable = true;
        gameObject.layer = 6;
    }
}
