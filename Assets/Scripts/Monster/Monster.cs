using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Monster : MonoBehaviour
{
    public int HP = 3;

    public float moveSpeed = 9.0f;

    public float recognitionRange = 100.0f;
    public float attackRange = 5.0f;
    public float haltRange = 2.0f;
    public Chase chase;

    #region MonsterShoot
    public int damage = 1;
    public float monsterShootTimer;
    public float monsterShootDelay = 0.7f;
    public float monsterReloadDelay = 2f;
    public bool isReloading = false;
    public int loadedBullet;
    public int magazineSize=3;
    #endregion

    #region Componets
    public Animator anim { get; private set; }
    public Rigidbody2D rigidBody { get; private set; }
    public SpriteRenderer spriteRenderer { get; private set; }
    public Collider2D col { get; private set; }
    #endregion

    #region States
    public MonsterStateMachine stateMachine { get; private set; }
    public MonsterIdleState idleState { get; private set; }
    public MonsterChaseState chaseState { get; private set; }
    public MonsterChaseAttackState chaseAttackState { get; private set; }
    public MonsterAttackState attackState { get; private set; }
    #endregion

    public GameObject player;
    public GameObject monsterBullet;
    public float tempSpeed;

    [Header("CameraSetting")]
    public CamShakeProfile profile;
    private CinemachineImpulseSource impulseSource;
    

    private void Awake()
    {
        stateMachine = new MonsterStateMachine();
        player = GameObject.FindWithTag("Player");
        idleState = new MonsterIdleState(this, stateMachine, player);
        chaseState = new MonsterChaseState(this, stateMachine, player);
        chaseAttackState = new MonsterChaseAttackState(this, stateMachine, player);
        attackState = new MonsterAttackState(this, stateMachine, player);
        tempSpeed = moveSpeed;
    }

    private void Start()
    {
        anim = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        chase = GetComponent<Chase>();
        stateMachine.Initialize(idleState);

        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    private void Update()
    {
        stateMachine.currentState.Update();
    }

    public void OnDamaged(int damage)
    {
        // 피격에 따른 카메라 진동
        CameraManager.instance.CameraShakeFromProfile(profile, impulseSource);

        HP -= damage;

        if (HP == 0)
        {
            Destroy(gameObject);
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            OnDamaged(1);
        }
        else if (collision.gameObject.CompareTag("Ground"))
        {
            //Debug.Log("in");
            //tempSpeed = 0f;
        }

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        //Debug.Log("out");
        //tempSpeed = moveSpeed;
    }

    public void Shoot()
    {
        if(loadedBullet > 0 && monsterShootTimer < 0.0)
        {
            monsterShootTimer = monsterShootDelay;
            loadedBullet--;

            //Create Bullet
            GameObject bulletObj = Instantiate(monsterBullet, transform.position, transform.rotation);
            Rigidbody2D rigid = bulletObj.GetComponent<Rigidbody2D>();
            MonsterBullet bullet = bulletObj.GetComponent<MonsterBullet>();

            Vector2 dir = player.transform.position - transform.position; 
            dir.Normalize();

            bullet.BulletInitialize(damage, dir);
        }
    }

    public void Reload()
    {
        isReloading = true;
        StartCoroutine(ReloadProcess());
    }

    IEnumerator ReloadProcess()
    {
        yield return new WaitForSeconds(monsterReloadDelay);
        loadedBullet = magazineSize;
        isReloading = false;
    }

}