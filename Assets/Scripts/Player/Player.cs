using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Cinemachine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public static Player instance = null;

    [Header("Life info")]
    public int curHP = 10;
    public int maxHP = 10;
    public int money = 0;
    public int shield = 0;
    [SerializeField] private float hitDuration = 0.6f;

    [Header("Move info")]
    public float moveSpeed = 5.65f;
    public float dashSpeed = 13.0f;
    public float dashDuration = 0.58f;
    public float dashExpCoefficient = -3.5f;

    [Header("Knockback info")]
    public Vector2 knockbackDir;
    public float knockbackMagnitude;
    public float knockbackExpCoefficient = -0.1f;

    [Header("Knockback Tempinfo")]
    public Vector2 knockbackDi2;
    public float knockbackMagnitude2;
    //Temp variable
    //public GameObject DeadUI;

    [Header("Gun info")]
    public Gun gun;
    public GameObject gunParent;
    public bool isAttackable = true;

    [Header("State Check")]
    public bool isCombatZone = false;
    public bool isStateChangeable = true;
    public bool isInteraction = false;
    public bool isDamaged = false;

    #region Componets
    //public Animator anim { get; private set; }
    public AnimController animController { get; private set; }

    public Rigidbody2D rb { get; private set; }
    public SpriteRenderer spriteRenderer { get; private set; }
    public Collider2D col { get; private set; }
    public PlayerHit playerHit { get; private set; }
    #endregion

    #region States
    public PlayerStateMachine stateMachine { get; private set; }
    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerDashState dashState { get; private set; }
    public PlayerKnockbackState knockbackState { get; private set; }

    #endregion

    [Header("CameraSetting")]
    public CamShakeProfile profile;
    public CameraManager cameraManager;
    private CinemachineImpulseSource impulseSource;

    private void Awake()
    {
        stateMachine = new PlayerStateMachine(this);

        idleState = new PlayerIdleState(this, stateMachine, AnimState.Idle);
        moveState = new PlayerMoveState(this, stateMachine, AnimState.Run);
        dashState = new PlayerDashState(this, stateMachine, AnimState.Wave);
        knockbackState = new PlayerKnockbackState(this, stateMachine, AnimState.knockBack);

        if (instance == null)
        { //생성 전이면
            instance = this; //생성
        }
        else if (instance != this)
        { //이미 생성되어 있으면
            Destroy(this.gameObject); //새로만든거 삭제
        }

        DontDestroyOnLoad(this.gameObject);

        //anim = GetComponentInChildren<Animator>();
        animController = GetComponentInChildren<AnimController>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        stateMachine.Initialize(idleState);
    }

    private void Start()
    {
        playerHit = GetComponentInChildren<PlayerHit>();

        cameraManager = FindObjectOfType<CameraManager>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    private void Update()
    {
        if(cameraManager == null) cameraManager = FindObjectOfType<CameraManager>();

        stateMachine.currentState.Update();

        //Check CombatZone
        if (isCombatZone && !gunParent.gameObject.activeSelf)
            gunParent.gameObject.SetActive(true);
        else if (!isCombatZone && gunParent.gameObject.activeSelf)
            gunParent.gameObject.SetActive(false);

        if (Input.GetKeyDown(KeyCode.L))
            PlayerKnockBack(knockbackDi2, knockbackMagnitude2);
    }

    public void SetVelocity(float _xVelocity, float _yVelocity)
    {
        rb.velocity = new Vector2(_xVelocity, _yVelocity);
    }

    public void SetVelocity(Vector2 vel)
    {
        rb.velocity = vel;
    }

    public void OnDamamged(int damage)
    {
        if(IsDash())
        {
            SkillManager.instance.RollingAdvantage();
            return;
        }    

        if(!isDamaged)
        {
            isDamaged = true;
            // monster에게 맞았을때 쉐이킹
            try
            {
                cameraManager.CameraShakeFromProfile(profile, impulseSource);
            }
            catch (System.NullReferenceException ex)
            {
                cameraManager = FindObjectOfType<CameraManager>();
                cameraManager.CameraShakeFromProfile(profile, impulseSource);
            }

            if (shield > 0)
                shield -= damage;
            else
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
                ChangePlayerLayer(7);
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
        ChangePlayerLayer(6);
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

    public void PlayerKnockBack(Vector2 dir, float mag)
    {
        knockbackDir = dir;
        knockbackDir.Normalize();
        knockbackMagnitude = mag;

        SetVelocity(0, 0);
        stateMachine.ChangeState(knockbackState);
    }

    public void ChangePlayerLayer(int layer)
    {
        gameObject.layer = layer;
        playerHit.gameObject.layer = layer;
    }

}
