using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    [Header("Life info")]
    public int HP = 3;
    [SerializeField] private float hitDuration;

    [Header("Skill info")]
    public float curMP = 100.0f;
    public float maxMP = 100.0f;

    [Header("Move info")]
    public float moveSpeed = 12.0f;
    public float dashSpeed = 24.0f;
    public float dashDuration = 2.0f;
    public Vector2 facingDir;

    public GameObject DeadUI;

    //Temp variable
    public float expCoefficient = -3.0f;
    public int dashMode = 0;
   
    [Header("Gun info")]
    public Gun gun;
    public bool isAttackable = true;

    [Header("State Check")]
    public bool isCombatZone = true;
    public bool isStateChangeable = true;

    #region Componets
    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public SpriteRenderer spriteRenderer { get; private set; }
    public Collider2D col { get; private set; }
    #endregion

    #region States
    public PlayerStateMachine stateMachine { get; private set; }
    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerDashState dashState { get; private set; }
    #endregion

    private void Awake()
    {
        stateMachine = new PlayerStateMachine(this);

        idleState = new PlayerIdleState(this, stateMachine, "Idle");
        moveState = new PlayerMoveState(this, stateMachine, "Move");
        dashState = new PlayerDashState(this, stateMachine, "Dash");

        if (ScenesManager.instance.GetSceneNum() >= 2)
            isCombatZone = true;
        else
            isCombatZone = false;
    }

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        stateMachine.Initialize(idleState);
    }

    private void Update()
    {
        //Debug.Log(stateMachine.currentState);
        stateMachine.currentState.Update();

        //if (isCombatZone)//To do. Check CombatZone
        //{
        //    //gun.SetActive(false)
        //}
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("MonsterBullet") || collision.gameObject.CompareTag("Monster"))
        {
            OnDamamged(1);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("MonsterBullet") || collision.gameObject.CompareTag("Monster"))
        {
            OnDamamged(1);
        }
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.DrawLine(groundCheck.position, new Vector3(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
    //}

    public void SetVelocity(float _xVelocity, float _yVelocity)
    {
        rb.velocity = new Vector2(_xVelocity, _yVelocity);
    }

    public void OnDamamged(int damage)
    {
        HP -= damage;
       // Debug.Log("HP: " + HP);
        if (HP <= 0)
        {
            Debug.Log("Player Dead");
            PlayerDead();
            Time.timeScale = 0.0f;
        }
        else
        {
            //Change Layer & Change Color
            gameObject.layer = 7;

            StartCoroutine(DamagedProcess());
        }
    }

    private void PlayerDead()
    {
        DeadUI.SetActive(true);
    }

    IEnumerator DamagedProcess()
    {
        for (int i = 0; i < 2; i++) 
        {
            spriteRenderer.color = new Color(1, 1, 1, 0.4f);
            yield return new WaitForSeconds(hitDuration / 4.0f);

            spriteRenderer.color = new Color(1, 1, 1, 1);
            yield return new WaitForSeconds(hitDuration / 4.0f);
        }
        gameObject.layer = 6;
    }
}
