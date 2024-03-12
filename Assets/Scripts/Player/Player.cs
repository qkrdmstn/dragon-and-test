using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    [Header("Life info")]
    [SerializeField] private int HP = 3;
    [SerializeField] private float hitDuration;

    [Header("Move info")]
    public float moveSpeed = 12.0f;
    public float dashSpeed = 24.0f;
    public float dashDuration = 2.0f;
    public float expCoefficient = -3.0f;
    public int dashMode = 0;

    [Header("Gun info")]
    public Gun gun;
    public bool isAttackable = true;

    //To do. facing direction으로 애니메이션 방향 정하기

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
        stateMachine = new PlayerStateMachine();

        idleState = new PlayerIdleState(this, stateMachine, "Idle");
        moveState = new PlayerMoveState(this, stateMachine, "Move");
        dashState = new PlayerDashState(this, stateMachine, "Dash");
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

        //To do. 전투 가능 지역 여부 판단 
        //gun.SetActive(false)
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

        if (HP == 0)
            Debug.Log("Player Dead");
        else
        {
            //Change Layer & Change Color
            gameObject.layer = 7;
            spriteRenderer.color = new Color(1, 1, 1, 0.4f);

            Invoke("OffDamaged", hitDuration);
        }
    }

    private void OffDamaged()
    {
        gameObject.layer = 6;
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }


}
