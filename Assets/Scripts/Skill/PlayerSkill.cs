using DG.Tweening.Plugins;
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
    public float UseSkill(SkillDB data)
    {
        SeotdaHwatuCombination code = data.TransStringToEnum();
        float cooTime = 0.0f;
        //Debug.Log(code + " damage:" + data.damage + " range:" + data.range + " force:" + data.force + " speed:" + data.speed);
        switch (code)
        {
            case SeotdaHwatuCombination.GTT38:
                cooTime = Superman(data.duration, data.coolTime);
                break;
            case SeotdaHwatuCombination.GTT18:
                break;
            case SeotdaHwatuCombination.GTT13:
                break;
            case SeotdaHwatuCombination.JTT:
                cooTime = SphereAttack(data.TransStringToEnum(), data.damage, data.speed, data.range, data.period, data.force, data.coolTime);
                break;
            case SeotdaHwatuCombination.TT9:
                cooTime = GuidedMissile(data.TransStringToEnum(), data.damage, data.speed, data.range, data.coolTime);
                break;
            case SeotdaHwatuCombination.TT8:
                cooTime = SokbakSkill(data.TransStringToEnum(), data.damage, data.range, data.speed, data.coolTime);
                break;
            case SeotdaHwatuCombination.TT7:
                cooTime = cooTime = BlankBullet(data.damage, data.range, data.force, data.coolTime);
                break;
            case SeotdaHwatuCombination.TT6:
                cooTime = GuidedMissile(data.TransStringToEnum(),data.damage, data.speed, data.range, data.coolTime);
                break;
            case SeotdaHwatuCombination.TT5:
                cooTime = BlankBullet(data.damage, data.range, data.force, data.coolTime);
                break;
            case SeotdaHwatuCombination.TT4:
                cooTime = SokbakSkill(data.TransStringToEnum(), data.damage, data.range, data.speed, data.coolTime);
                break;
            case SeotdaHwatuCombination.TT3:
                cooTime = FlameThrower(data.TransStringToEnum(), data.damage, data.duration, data.period, data.coolTime);
                break;
            case SeotdaHwatuCombination.TT2:
                cooTime = FlameThrower(data.TransStringToEnum(), data.damage, data.duration, data.period, data.coolTime);
                break;
            case SeotdaHwatuCombination.TT1:
                cooTime = SphereAttack(data.TransStringToEnum(), data.damage, data.speed, data.range, data.period, data.force, data.coolTime);
                break;
            case SeotdaHwatuCombination.KK9:
                cooTime = DashSkill(data.range, data.speed, data.coolTime);
                break;
            case SeotdaHwatuCombination.KK8:
                cooTime = DashSkill(data.range, data.speed, data.coolTime);
                break;
            case SeotdaHwatuCombination.KK7:
                cooTime = DashSkill(data.range, data.speed, data.coolTime);
                break;
            case SeotdaHwatuCombination.KK6:
                cooTime = DashSkill(data.range, data.speed, data.coolTime);
                break;
            case SeotdaHwatuCombination.KK5:
                cooTime = DashSkill(data.range, data.speed, data.coolTime);
                break;
            case SeotdaHwatuCombination.KK4:
                cooTime = DashSkill(data.range, data.speed, data.coolTime);
                break;
            case SeotdaHwatuCombination.KK3:
                cooTime = DashSkill(data.range, data.speed, data.coolTime);
                break;
            case SeotdaHwatuCombination.KK2:
                cooTime = DashSkill(data.range, data.speed, data.coolTime);
                break;
            case SeotdaHwatuCombination.KK1:
                cooTime = ReinforceAttack(data.duration, data.coolTime);
                break;
            case SeotdaHwatuCombination.KK0:
                break;
            case SeotdaHwatuCombination.blank:
                break;
        }
        return cooTime;
    }

    #region Active
    #region BlankBullet
    private float BlankBullet(int damage, float impactRadius, float impactForce, float coolTime)
    {
        _impactRadius = impactRadius;

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

        return coolTime;
    }
    #endregion

    #region Dash
    private float DashSkill(float dist, float speed, float coolTime)
    {
        StartCoroutine(DashSkillCoroutine(dist, speed));

        return coolTime;
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
    private float SokbakSkill(SeotdaHwatuCombination code, int damage, float dist, float projectileSpeed, float coolTime)
    {
        //방위
        //int[] dx = { 0, 1, 0, -1, -1, 1, 1, -1 };
        //int[] dy = { 1, 0, -1, 0, 1, 1, -1, -1 };
        //float[] degree = { 90.0f, 0.0f, -90.0f, 180.0f, 135.0f, 45.0f, -45.0f, -135.0f};

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

        return coolTime;
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
    Coroutine flameCoroutine;
    GameObject flameObject;
    private float FlameThrower(SeotdaHwatuCombination code, int damage, float duration, float period, float coolTime)
    {
        if (flameCoroutine != null)
        {
            return 0.0f;
        }

        flameCoroutine = StartCoroutine(FlameThrowerCoroutine(code, damage, duration, period));

        return coolTime;
    }

    IEnumerator FlameThrowerCoroutine(SeotdaHwatuCombination code, int damage, float duration, float period)
    {
        Player.instance.isAttackable = false;

        float timer = duration;

        GameObject prefabs = skillObjDictionary[code];
        flameObject = Instantiate(prefabs, transform.position, Quaternion.identity);
        SkillObj_Flame flame = flameObject.GetComponent<SkillObj_Flame>();
        flame.Initialize(damage, new Vector2(0,0), StatusEffect.None, period);
        while (timer >= 0.0f)
        {
            timer -= Time.deltaTime;

            yield return null;
        }
        Destroy(flameObject);
        Player.instance.isAttackable = true;
    }
    #endregion

    #region GuidedMissile 
    private float GuidedMissile(SeotdaHwatuCombination code, int damage, float speed, float range, float coolTime)
    {
        SkillObj_Missile[] beforeObj = FindObjectsByType<SkillObj_Missile>(FindObjectsSortMode.None);
        if(beforeObj.Length != 0)
        {
            return 0.0f;
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

        return coolTime;
    }

    //IEnumerator GuidedMissileCoroutine(SeotdaHwatuName name, int damage, float speed, float duration, int numProjectile)
    //{

    //}


    #endregion

    #region SphereAttack 
    private float SphereAttack(SeotdaHwatuCombination code, int damage, float speed, float range, float period, float force, float coolTime)
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

        return coolTime;
    }
    #endregion

    #region SuperMan
    private float Superman(float duration, float coolTime)
    {
        StartCoroutine(SupermanCoroutine(duration));

        return coolTime;
    }

    IEnumerator SupermanCoroutine(float duration)
    {
        SkillManager.instance.ClearCoolTimer();
        Player.instance.isSuperman = true;
        yield return new WaitForSeconds(duration);
        Player.instance.isSuperman = false;
    }

    #endregion

    #region Reinforce
    private float ReinforceAttack(float duration, float coolTime)
    {
        StartCoroutine(ReinforceAttackCoroutine(duration));

        return coolTime;
    }

    IEnumerator ReinforceAttackCoroutine(float duration)
    {
        Player.instance.reinforceAttack += 1;
        yield return new WaitForSeconds(duration);
        Player.instance.reinforceAttack -= 1;
    }
    #endregion

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
