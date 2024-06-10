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

    public GameObject[] skillPrefabs;
    Dictionary<SeotdaHwatuName, GameObject> skillObjDictionary = new Dictionary<SeotdaHwatuName, GameObject>();

    #region Components
    private Player player;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        skillPrefabs = Resources.LoadAll<GameObject>("Prefabs/Skill");
        for(int i=0; i<skillPrefabs.Length; i++)
        {
            string objectName = skillPrefabs[i].name;
            for (int j=0; j<20; j++)
            {
                string cardName = ((SeotdaHwatuName)j).ToString();
                if (objectName.Contains(cardName))
                {
                    skillObjDictionary.Add((SeotdaHwatuName)j, skillPrefabs[i]);
                    break;
                }
            }
        }
        player = GetComponent<Player>();

        impactLayerMask = LayerMask.GetMask("Monster", "MonsterBullet");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            //SokbakSkill(SeotdaHwatuName.AprCuckoo, 5, 6, 15);
            FlameThrower(SeotdaHwatuName.FebBird, 1, 5, 0.3f);
            //Flooring(SeotdaHwatuName.JanCrane, 1, 10, 15f, 1.5f);

        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            Flooring(SeotdaHwatuName.MayBridge, 1, 10, 15f, 1.5f);
            //FlameThrower(SeotdaHwatuName.MarCherryLight, 1, 5, 0.3f);
        }
        //if(Input.GetKeyDown(KeyCode.J))
        //{
        //    BlankBullet(2, 6, 30);
        //}
    }
    public void UseSkill(SeotdaHwatuName name, int damage, float range, float force, float duration, float speed, float period)
    {
        Debug.Log(name + " damage:" + damage + " range:" + range + " force:" + force + " speed:" + speed);
        switch(name)
        {
            //Main Card
            case SeotdaHwatuName.JanCrane:
                Flooring(name, damage, duration, speed, period);
                break;
            case SeotdaHwatuName.FebBird:
                FlameThrower(name, damage, duration, period);
                break;
            case SeotdaHwatuName.MarCherryLight:
                FlameThrower(name, damage, duration, period);
                break;
            case SeotdaHwatuName.AprCuckoo:
                SokbakSkill(name, damage, range, speed);
                break;
            case SeotdaHwatuName.MayBridge:
                Flooring(name, damage, duration, speed, period);
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

    #region BlankBullet
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

                if(monster != null)
                {
                    Vector2 impactDir = target.transform.position - this.transform.position;
                    impactDir.Normalize();

                    monster.Knockback(impactDir, impactForce);
                    monster.OnDamaged(damage);
                }
            }
        }
    }
    #endregion

    #region Dash
    private void DashSkill(float dist, float speed)
    {
        StartCoroutine(DashSkillCoroutine(dist, speed));
    }

    IEnumerator DashSkillCoroutine(float dist, float speed)
    {
        //Change Layer & Change Color
        player.ChangePlayerLayer(14);
        player.SetIdleStatePlayer();
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
    #endregion

    #region Sokbak
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

            GameObject prefabs = skillObjDictionary[name];
            GameObject projectilObj = Instantiate(prefabs, transform.position, Quaternion.Euler(0, 0, degree[i]));
            SkillObj_Projectile projectile = projectilObj.GetComponent<SkillObj_Projectile>();
            Debug.Log(projectile);
            projectile.Initialize(0, dist, dir, projectileSpeed, StatusEffect.Sokbak);
        }
    }
    #endregion

    #region Flooring
    private void Flooring(SeotdaHwatuName name, int damage, float duration, float speed, float period)
    {
        StartCoroutine(FlooringCoroutine(name, damage, duration, speed, period));
    }

    IEnumerator FlooringCoroutine(SeotdaHwatuName name, int damage, float duration, float speed, float period)
    {
        //Initial Direction Setting
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = mousePos - this.transform.position;
        dir.Normalize();
        float theta = Vector2.Angle(Vector2.right, dir);
        if (dir.y < 0)
            theta *= -1;

        //Create Object
        GameObject prefabs = skillObjDictionary[name];
        GameObject Obj = Instantiate(prefabs, transform.position, Quaternion.Euler(0,0, theta));
        SkillObj_Flooring flame = Obj.GetComponent<SkillObj_Flooring>();
        flame.Initialize(damage, dir, mousePos, speed, StatusEffect.Slow, period);

        float timer = duration;
        while (timer >= 0.0f)
        {
            timer -= Time.deltaTime;
            yield return null;
        }
        Destroy(Obj);
    }
    #endregion

    #region flame
    private void FlameThrower(SeotdaHwatuName name, int damage, float duration, float period)
    {
        StartCoroutine(FlameThrowerCoroutine(name, damage, duration, period));
    }

    IEnumerator FlameThrowerCoroutine(SeotdaHwatuName name, int damage, float duration, float period)
    {
        player.isAttackable = false;

        float timer = duration;


        GameObject prefabs = skillObjDictionary[name];
        GameObject Obj = Instantiate(prefabs, transform.position, Quaternion.identity);
        SkillObj_Flame flame = Obj.GetComponent<SkillObj_Flame>();
        flame.Initialize(damage, new Vector2(0,0), StatusEffect.None, period);
        while (timer >= 0.0f)
        {
            timer -= Time.deltaTime;

            yield return null;
        }
        Destroy(Obj);
        player.isAttackable = true;
    }
    #endregion

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(this.transform.position, _impactRadius);
    //}
}
