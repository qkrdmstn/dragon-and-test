using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Cinemachine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [Header("Life info")]
    public int curHP = 3;
    public int maxHP = 3;
    [SerializeField] private float hitDuration;

    [Header("Skill info")]
    public int blankBulletNum = 4;
    public float curMP = 100.0f;
    public float maxMP = 100.0f;

    [Header("Move info")]
    public float moveSpeed = 12.0f;
    public float dashSpeed = 24.0f;
    public float dashDuration = 2.0f;
    public float expCoefficient = -3.0f;

    //Temp variable
    //public GameObject DeadUI;
   
    [Header("Gun info")]
    public Gun gun;
    public GameObject gunParent;
    public bool isAttackable = true;

    [Header("State Check")]
    public bool isCombatZone = true;
    public bool isStateChangeable = true;
    public bool isInteraction = false;
    public bool isDamaged = false;

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

    [Header("CameraSetting")]
    public CamShakeProfile profile;
    public CameraManager cameraManager;
    private CinemachineImpulseSource impulseSource;

    private void Awake()
    {
        stateMachine = new PlayerStateMachine(this);

        idleState = new PlayerIdleState(this, stateMachine, "Idle");
        moveState = new PlayerMoveState(this, stateMachine, "Move");
        dashState = new PlayerDashState(this, stateMachine, "Dash");

        if (SceneManager.GetActiveScene().name == "Battle_1"
            || SceneManager.GetActiveScene().name == "Puzzle_1"
            || SceneManager.GetActiveScene().name == "Tutorial")
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

        cameraManager = FindObjectOfType<CameraManager>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    private void Update()
    {
        //Debug.Log(stateMachine.currentState);
        stateMachine.currentState.Update();

        //Check CombatZone
        if (isCombatZone && !gunParent.gameObject.activeSelf)
            gunParent.gameObject.SetActive(true);
        else if (!isCombatZone && gunParent.gameObject.activeSelf)
            gunParent.gameObject.SetActive(false);
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
        if(!isDamaged)
        {
            isDamaged = true;
            // monster에게 맞았을때 쉐이킹 
            cameraManager.CameraShakeFromProfile(profile, impulseSource);

            curHP -= damage;
            // Debug.Log("HP: " + HP);
            if (curHP <= 0)
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
    }

    private void PlayerDead()
    {
        UIManager.instance.SceneUI["Battle_1"].GetComponent<BattleUIGroup>().childUI[0].SetActive(true);
        //DeadUI.SetActive(true);
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
        isDamaged = false;
    }

    public void SetIdleStatePlayer()
    {
        SetVelocity(0, 0);
        stateMachine.ChangeState(idleState);
    }

    public bool IsDash()
    {
        if (stateMachine.currentState == dashState)
            return true;
        return false;

    }
}
