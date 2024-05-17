using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PlayerSkill : MonoBehaviour
{
    //public IObjectPool<GameObject> pool { get; set; }

    [Header("Skill info")]
    [Space(10f)]
    private int impactLayerMask;
    float _impactRadius;

    public GameObject[] projectilePrefabs;
    Dictionary<SeotdaHwatuName, GameObject> projectileDictionary = new Dictionary<SeotdaHwatuName, GameObject>();

    #region Components
    private Player player;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        projectilePrefabs = Resources.LoadAll<GameObject>("Prefabs/SkillProjectile");
        for(int i=0; i<projectilePrefabs.Length; i++)
        {
            string objectName = projectilePrefabs[i].name;
            for (int j=0; j<20; j++)
            {
                string cardName = ((SeotdaHwatuName)j).ToString();
                if (objectName.Contains(cardName))
                {
                    projectileDictionary.Add((SeotdaHwatuName)j, projectilePrefabs[i]);
                    break;
                }
            }
        }
        player = GetComponent<Player>();

        impactLayerMask = LayerMask.GetMask("Monster", "MonsterBullet");
    }

    private void Update()
    {
        //if(Input.GetKeyDown(KeyCode.G))
        //{
        //    SokbakSkill(SeotdaHwatuName.AprCuckoo, 5, 6, 15);
        //}
        if (Input.GetKeyDown(KeyCode.H))
        {
            //SokbakSkill(SeotdaHwatuName.AugMoon, 5, 6, 15);
            DashSkill(7, 40);
        }
        //if(Input.GetKeyDown(KeyCode.J))
        //{
        //    BlankBullet(2, 6, 30);
        //}
    }
    public void UseSkill(SeotdaHwatuName name, int damage, float range, float force, float speed)
    {
        Debug.Log(name + " damage:" + damage + " range:" + range + " force:" + force + " speed:" + speed);
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
                SokbakSkill(name, damage, range, speed);
                break;
            case SeotdaHwatuName.MayBridge:
                break;
            case SeotdaHwatuName.JunButterfly:
                BlankBullet(damage, range, force);
                break;
            case SeotdaHwatuName.JulBoar:
                break;
            case SeotdaHwatuName.AugMoon:
                SokbakSkill(name, damage, range, speed);
                break;
            case SeotdaHwatuName.SepSakajuki:
                BlankBullet(damage, range, force);
                break;
            case SeotdaHwatuName.OctDeer:
                break;
            //Sub Card
            case SeotdaHwatuName.JanPine:
                DashSkill(range, speed);
                break;
            case SeotdaHwatuName.FebPrunus:
                DashSkill(range, speed);
                break;
            case SeotdaHwatuName.MarCherry:
                DashSkill(range, speed);
                break;
            case SeotdaHwatuName.AprWisteria:
                DashSkill(range, speed);
                break;
            case SeotdaHwatuName.MayIris:
                DashSkill(range, speed);
                break;
            case SeotdaHwatuName.JunPeony:
                DashSkill(range, speed);
                break;
            case SeotdaHwatuName.JulLespedeza:
                DashSkill(range, speed);
                break;
            case SeotdaHwatuName.AugGoose:
                DashSkill(range, speed);
                break;
            case SeotdaHwatuName.SepChrysanthemum:
                DashSkill(range, speed);
                break;
            case SeotdaHwatuName.OctFoliage:
                DashSkill(range, speed);
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
                MonsterPool.instance.pool.Release(inRangeTarget[i].gameObject);
                

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

    private void DashSkill(float dist, float speed)
    {
        StartCoroutine(DashSkillCoroutine(dist, speed));
    }

    IEnumerator DashSkillCoroutine(float dist, float speed)
    {
        //Change Layer & Change Color
        player.ChangePlayerLayer(14);
        player.isStateChangeable = false;
        player.isAttackable = false;


        float curDist = 0.0f;
        //Initial Direction Setting
        Vector2 dir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - this.transform.position;
        dir.Normalize();

        while (dist >= curDist)
        {
            float dv = Time.deltaTime * speed;
            Vector2 dashVel = speed * dir;
            player.SetVelocity(dashVel);
            curDist += dv;
            yield return new WaitForFixedUpdate();
        }
        player.SetVelocity(0, 0);

        player.ChangePlayerLayer(6);
        player.isStateChangeable = true;
        player.isAttackable = true;
    }

    private void SokbakSkill(SeotdaHwatuName name, int damage, float dist, float projectileSpeed)
    {
        //방위
        int[] dx = { 0, 1, 0, -1, -1, 1, 1, -1 };
        int[] dy = { 1, 0, -1, 0, 1, 1, -1, -1 };
        float[] degree = { 90.0f, 0.0f, -90.0f, 180.0f, 135.0f, 45.0f, -45.0f, -135.0f};

        int numProjectile = 0;
        if (name == SeotdaHwatuName.AprCuckoo)
            numProjectile = 4;
        else if (name == SeotdaHwatuName.AugMoon)
            numProjectile = 8;
        
        for(int i=0; i< numProjectile; i++)
        {
            Vector2 dir = new Vector2(dx[i], dy[i]);
            dir.Normalize();

            GameObject prefabs = projectileDictionary[name];
            GameObject projectilObj = Instantiate(prefabs, transform.position, Quaternion.Euler(0, 0, degree[i]));
            SkillProjectile projectile = projectilObj.GetComponent<SkillProjectile>();

            projectile.Initialize(0, dist, dir, projectileSpeed, StatusEffect.Sokbak);
        }
    }
}
