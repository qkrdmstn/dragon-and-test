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
    Dictionary<SeotdaHwatuCombination, GameObject> skillObjDictionary = new Dictionary<SeotdaHwatuCombination, GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        skillPrefabs = Resources.LoadAll<GameObject>("Prefabs/Skill");
        for(int i=0; i<skillPrefabs.Length; i++)
        {
            string objectName = skillPrefabs[i].name;
            for (int j=0; j<33; j++)
            {
                string cardName = ((SeotdaHwatuCombination)j).ToString();
                if (objectName.Contains(cardName))
                {
                    skillObjDictionary.Add((SeotdaHwatuCombination)j, skillPrefabs[i]);
                    break;
                }
            }
        }

        impactLayerMask = LayerMask.GetMask("Monster", "MonsterBullet");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            //SokbakSkill(SeotdaHwatuName.AprCuckoo, 5, 6, 15);
            //FlameThrower(SeotdaHwatuName.FebBird, 1, 5, 0.3f);
            //Flooring(SeotdaHwatuName.JanCrane, 1, 10, 15f, 1.5f);
            //GuidedMissile(SeotdaHwatuCombination.TT9, 1, 10, 10, 2.5f);

        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            //Flooring(SeotdaHwatuName.MayBridge, 1, 10, 15f, 1.5f);
            //FlameThrower(SeotdaHwatuCombination.TT3, 1, 5, 0.3f);
            //GuidedMissile(SeotdaHwatuName.JulBoar, 1, 10, 10, 2.5f);
        }
        //if(Input.GetKeyDown(KeyCode.J))
        //{
        //    BlankBullet(2, 6, 30);
        //}
    }
    public void UseSkill(SkillDB data)
    {
        SeotdaHwatuCombination code = data.TransStringToEnum();
        //Debug.Log(code + " damage:" + data.damage + " range:" + data.range + " force:" + data.force + " speed:" + data.speed);
        switch (code)
        {
            case SeotdaHwatuCombination.GTT38:
                break;
            case SeotdaHwatuCombination.GTT18:
                break;
            case SeotdaHwatuCombination.GTT13:
                break;
            case SeotdaHwatuCombination.JTT:
                SphereAttack(data.TransStringToEnum(), data.damage, data.speed, data.range, data.period, data.force);
                break;
            case SeotdaHwatuCombination.TT9:
                GuidedMissile(data.TransStringToEnum(), data.damage, data.speed, data.duration, data.range);
                break;
            case SeotdaHwatuCombination.TT8:
                SokbakSkill(data.TransStringToEnum(), data.damage, data.range, data.speed);
                break;
            case SeotdaHwatuCombination.TT7:
                BlankBullet(data.damage, data.range, data.force);
                break;
            case SeotdaHwatuCombination.TT6:
                GuidedMissile(data.TransStringToEnum(),data.damage, data.speed, data.duration, data.range);
                break;
            case SeotdaHwatuCombination.TT5:
                BlankBullet(data.damage, data.range, data.force);
                break;
            case SeotdaHwatuCombination.TT4:
                SokbakSkill(data.TransStringToEnum(), data.damage, data.range, data.speed);
                break;
            case SeotdaHwatuCombination.TT3:
                FlameThrower(data.TransStringToEnum(), data.damage, data.duration, data.period);
                break;
            case SeotdaHwatuCombination.TT2:
                FlameThrower(data.TransStringToEnum(), data.damage, data.duration, data.period);
                break;
            case SeotdaHwatuCombination.TT1:
                SphereAttack(data.TransStringToEnum(), data.damage, data.speed, data.range, data.period, data.force);
                break;
            case SeotdaHwatuCombination.AL12:
                break;
            case SeotdaHwatuCombination.DS14:
                break;
            case SeotdaHwatuCombination.GPP19:
                break;
            case SeotdaHwatuCombination.JPP110:
                break;
            case SeotdaHwatuCombination.JS410:
                break;
            case SeotdaHwatuCombination.SR46:
                break;
            case SeotdaHwatuCombination.AHES74:
                break;
            case SeotdaHwatuCombination.TTCatch73:
                break;
            case SeotdaHwatuCombination.MTGR94:
                break;
            case SeotdaHwatuCombination.KK9:
                DashSkill(data.range, data.speed);
                break;
            case SeotdaHwatuCombination.KK8:
                DashSkill(data.range, data.speed);
                break;
            case SeotdaHwatuCombination.KK7:
                DashSkill(data.range, data.speed);
                break;
            case SeotdaHwatuCombination.KK6:
                DashSkill(data.range, data.speed);
                break;
            case SeotdaHwatuCombination.KK5:
                DashSkill(data.range, data.speed);
                break;
            case SeotdaHwatuCombination.KK4:
                DashSkill(data.range, data.speed);
                break;
            case SeotdaHwatuCombination.KK3:
                DashSkill(data.range, data.speed);
                break;
            case SeotdaHwatuCombination.KK2:
                DashSkill(data.range, data.speed);
                break;
            case SeotdaHwatuCombination.KK1:
                StartCoroutine(ReinforceAttack(data.duration));
                break;
            case SeotdaHwatuCombination.KK0:
                break;
            case SeotdaHwatuCombination.blank:
                break;
        }

    }

    #region Active
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
                Boss boss = target.GetComponent<Boss>();

                if (monster != null)
                {
                    Vector2 impactDir = target.transform.position - this.transform.position;
                    impactDir.Normalize();

                    monster.Knockback(impactDir, impactForce);
                    monster.OnDamaged(damage);
                }
                else if (boss != null)
                {
                    boss.OnDamaged(damage);
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
        Player.instance.ChangePlayerLayer(14);
        Player.instance.SetIdleStatePlayer();
        Player.instance.isStateChangeable = false;
        Player.instance.isAttackable = false;

        float curDist = 0.0f;
        //Initial Direction Setting
        Vector2 dir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - this.transform.position;
        dir.Normalize();

        while (dist >= curDist)
        {
            float dv = Time.deltaTime * speed;
            Vector2 dashVel = speed * dir;
            Player.instance.SetVelocity(dashVel);
            curDist += dv;
            yield return new WaitForFixedUpdate();
        }
        Player.instance.SetVelocity(0, 0);

        Player.instance.ChangePlayerLayer(6);
        Player.instance.isStateChangeable = true;
        Player.instance.isAttackable = true;
    }
    #endregion

    #region Sokbak
    private void SokbakSkill(SeotdaHwatuCombination code, int damage, float dist, float projectileSpeed)
    {
        //방위
        int[] dx = { 0, 1, 0, -1, -1, 1, 1, -1 };
        int[] dy = { 1, 0, -1, 0, 1, 1, -1, -1 };
        float[] degree = { 90.0f, 0.0f, -90.0f, 180.0f, 135.0f, 45.0f, -45.0f, -135.0f};
        
        //if (code == SeotdaHwatuCombination.TT4)
        {
            Vector2 dir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - this.transform.position;
            dir.Normalize();
            float theta = Vector2.Angle(Vector2.right, dir);
            if (dir.y < 0)
                theta *= -1;

            GameObject prefabs = skillObjDictionary[code];
            GameObject projectilObj = Instantiate(prefabs, transform.position, Quaternion.Euler(0, 0, theta));
            SkillObj_Projectile projectile = projectilObj.GetComponent<SkillObj_Projectile>();
            Debug.Log(projectile);
            projectile.Initialize(damage, dist, dir, projectileSpeed, StatusEffect.Sokbak);
        }
        //else if (code == SeotdaHwatuCombination.TT8)
        //{
        //    int numProjectile = 4;
        //    for (int i = 0; i < numProjectile; i++)
        //    {
        //        Vector2 dir = new Vector2(dx[i], dy[i]);
        //        dir.Normalize();

        //        GameObject prefabs = skillObjDictionary[code];
        //        GameObject projectilObj = Instantiate(prefabs, transform.position, Quaternion.Euler(0, 0, degree[i]));
        //        SkillObj_Projectile projectile = projectilObj.GetComponent<SkillObj_Projectile>();
        //        Debug.Log(projectile);
        //        projectile.Initialize(0, dist, dir, projectileSpeed, StatusEffect.Sokbak);
        //    }
        //}
    }
    #endregion

    //#region Flooring
    //private void Flooring(SeotdaHwatuName name, int damage, float duration, float speed, float period)
    //{
    //    StartCoroutine(FlooringCoroutine(name, damage, duration, speed, period));
    //}

    //IEnumerator FlooringCoroutine(SeotdaHwatuName name, int damage, float duration, float speed, float period)
    //{
    //    //Initial Direction Setting
    //    Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //    Vector2 dir = mousePos - this.transform.position;
    //    dir.Normalize();
    //    float theta = Vector2.Angle(Vector2.right, dir);
    //    if (dir.y < 0)
    //        theta *= -1;

    //    //Create Object
    //    GameObject prefabs = skillObjDictionary[name];
    //    GameObject Obj = Instantiate(prefabs, transform.position, Quaternion.Euler(0,0, theta));
    //    SkillObj_Flooring flame = Obj.GetComponent<SkillObj_Flooring>();
    //    flame.Initialize(damage, dir, mousePos, speed, StatusEffect.Slow, period);

    //    float timer = duration;
    //    while (timer >= 0.0f)
    //    {
    //        timer -= Time.deltaTime;
    //        yield return null;
    //    }
    //    Destroy(Obj);
    //}
    //#endregion

    #region Flame
    private void FlameThrower(SeotdaHwatuCombination code, int damage, float duration, float period)
    {
        StartCoroutine(FlameThrowerCoroutine(code, damage, duration, period));
    }

    IEnumerator FlameThrowerCoroutine(SeotdaHwatuCombination code, int damage, float duration, float period)
    {
        Player.instance.isAttackable = false;

        float timer = duration;


        GameObject prefabs = skillObjDictionary[code];
        GameObject Obj = Instantiate(prefabs, transform.position, Quaternion.identity);
        SkillObj_Flame flame = Obj.GetComponent<SkillObj_Flame>();
        flame.Initialize(damage, new Vector2(0,0), StatusEffect.None, period);
        while (timer >= 0.0f)
        {
            timer -= Time.deltaTime;

            yield return null;
        }
        Destroy(Obj);
        Player.instance.isAttackable = true;
    }
    #endregion

    #region GuidedMissile 
    private void GuidedMissile(SeotdaHwatuCombination code, int damage, float speed, float duration, float range)
    {
        SkillObj_Missile[] beforeObj = FindObjectsByType<SkillObj_Missile>(FindObjectsSortMode.None);
        for(int i=0; i<beforeObj.Length; i++)
        {
            Destroy(beforeObj[i]);
        }
        
        int numProjectile = 0;
        if (code == SeotdaHwatuCombination.TT9)
            numProjectile = 9;
        else if (code == SeotdaHwatuCombination.TT6)
            numProjectile = 6;

        GameObject prefab = skillObjDictionary[code];
        for(int i=0; i< numProjectile; i++)
        {
            float angle = 360.0f / numProjectile * (i - numProjectile / 2);
            Vector2 direction = Quaternion.AngleAxis(angle, Vector3.forward) * new Vector2(1,0);
            float theta = Vector2.Angle(Vector2.right, direction);
            if (direction.y < 0)
                theta *= -1;

            direction.Normalize();
            Vector3 pos = transform.position + (Vector3)(direction);
            GameObject projectile = Instantiate(prefab, pos, Quaternion.Euler(0,0, theta));

            SkillObj_Missile missile = projectile.GetComponent<SkillObj_Missile>();
            missile.Initialize(damage, direction, StatusEffect.None, speed, range, theta);
        }
    }

    //IEnumerator GuidedMissileCoroutine(SeotdaHwatuName name, int damage, float speed, float duration, int numProjectile)
    //{

    //}


    #endregion

    #region SphereAttack 
    private void SphereAttack(SeotdaHwatuCombination code, int damage, float speed, float range, float period, float force)
    {
        GameObject prefab = skillObjDictionary[code];

        Vector2 dir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - this.transform.position;
        dir.Normalize();
        float theta = Vector2.Angle(Vector2.right, dir);
        if (dir.y < 0)
            theta *= -1;

        GameObject prefabs = skillObjDictionary[code];
        GameObject projectilObj = Instantiate(prefabs, transform.position, Quaternion.Euler(0, 0, theta));
        SkillObj_Sphere projectile = projectilObj.GetComponent<SkillObj_Sphere>();
        Debug.Log(projectile);
        projectile.Initialize(damage, dir, speed, range, StatusEffect.None, period, force);
    }
    #endregion

    IEnumerator ReinforceAttack(float duration)
    {
        Player.instance.reinforceAttack += 1;
        yield return new WaitForSeconds(duration);
        Player.instance.reinforceAttack -= 1;
    }
    #endregion

    #region Passive
    public void AL12Effect(GameObject pivotMonster, int damage)
    {
        SkillDB al12Data = SkillManager.instance.GetSkillDB(SeotdaHwatuCombination.AL12);
        Collider2D[] inRangeTarget = Physics2D.OverlapCircleAll(pivotMonster.transform.position, al12Data.range, LayerMask.GetMask("Monster"));
        for (int i = 0; i < inRangeTarget.Length; i++)
        {
            GameObject target = inRangeTarget[i].gameObject;
            if (target == pivotMonster)
                continue;

            MonsterBase monster = target.GetComponent<MonsterBase>();
            Boss boss = target.GetComponent<Boss>();
            if (monster != null) //몬스터일 경우,
            {
                monster.OnDamaged(damage);
            }
            else if (boss != null) //보스일 경우.
            {
                boss.OnDamaged(damage);
            }
        }
    }
    #endregion

    //private void OnDrawGizmos()
    //{
    //    SkillDB al12Data = SkillManager.instance.GetSkillDB(SeotdaHwatuCombination.AL12);

    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(this.transform.position, al12Data.range);
    //}
}
