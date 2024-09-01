using System.Collections;
using UnityEngine;
using Cinemachine;

public class Player : MonoBehaviour
{
    #region Value
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

    [Header("Gun info")]
    public Gun gun;
    public GameObject gunParent;
    public bool isAttackable = true;
    public int reinforceAttack = 0;

    [Header("State Check")]
    public bool isCombatZone = false;
    public bool isStateChangeable = true;
    public bool isInteraction = false;
    public bool isDamaged = false;
    public bool isFall = false;
    public bool isBounded = false;
    public bool isDead = false;
    #region Componets
    public PlayerAnimController animController { get; private set; }

    public Rigidbody2D rb { get; private set; }
    public Collider2D col { get; private set; }
    public PlayerHit playerHit { get; private set; }
    #endregion

    #region States
    public PlayerStateMachine stateMachine { get; private set; }
    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerDashState dashState { get; private set; }
    public PlayerKnockbackState knockbackState { get; private set; }
    public PlayerFallState fallState { get; private set; }

    #endregion

    [Header("CameraSetting")]
    public CamShakeProfile profile;
    public CameraManager cameraManager;
    private CinemachineImpulseSource impulseSource;
    #endregion

    private void Awake()
    {
        stateMachine = new PlayerStateMachine(this);

        idleState = new PlayerIdleState(this, stateMachine, PlayerAnimState.Idle);
        moveState = new PlayerMoveState(this, stateMachine, PlayerAnimState.Run);
        dashState = new PlayerDashState(this, stateMachine, PlayerAnimState.Wave);
        knockbackState = new PlayerKnockbackState(this, stateMachine, PlayerAnimState.knockBack);
        fallState = new PlayerFallState(this, stateMachine, PlayerAnimState.Fall);

        if (instance == null)
        { //생성 전이면
            instance = this; //생성
        }
        else if (instance != this)
        { //이미 생성되어 있으면
            Destroy(this.gameObject); //새로만든거 삭제
        }

        DontDestroyOnLoad(this.gameObject);

        animController = GetComponentInChildren<PlayerAnimController>();

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

    public Vector2 GetVelocity()
    {
        return rb.velocity;
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

            if (curHP <= 0) //Dead
            {
                if(SkillManager.instance.PassiveCheck(SeotdaHwatuCombination.JS410))
                { //4% 확률로 죽음 회피 & 체력 회복
                    float prob = 0.04f;
                    float randomVal = Random.Range(0.0f, 1.0f);
                    if (randomVal <= prob)
                        curHP = maxHP;
                }
                else
                {
                    PlayerDead();
                }
            }
            else if (SkillManager.instance.PassiveCheck(SeotdaHwatuCombination.AL12))
            { //피격 시 12%로 2초간 무적
                float prob = 0.12f;
                float randomVal = Random.Range(0.0f, 1.0f);
                if (randomVal <= prob)
                {
                    //Change Layer & Change Color
                    ChangePlayerLayer(7);
                    StartCoroutine(DamagedProcess(2.0f));
                }
                else
                {
                    //Change Layer & Change Color
                    ChangePlayerLayer(7);
                    StartCoroutine(DamagedProcess(hitDuration));
                }

            }
            else
            {
                //Change Layer & Change Color
                ChangePlayerLayer(7);
                StartCoroutine(DamagedProcess(hitDuration));
            }
        }
    }

    private void PlayerDead()
    {
        isDead = true;
        GameManager.instance.SetTimeScale(0f);
        UIManager.instance.SceneUI["Battle_1"].GetComponent<BattleUIGroup>().childUI[0].SetActive(true);
    }

    public void ReloadPlayer()
    {   
        isAttackable = false;
        isCombatZone = false;
        isDead = false;
        isDamaged = false;
        curHP = maxHP;
        money = 0;

        SkillManager.instance.ClearSkill(); // 모든 화투, 스킬 삭제
        animController.isBreath = false;
        animController.SetAnim(PlayerAnimState.Idle);
    }

    IEnumerator DamagedProcess(float duration)
    {
        for (int i = 0; i < 2; i++) 
        {
            animController.SetMaterialColor(new Color(1, 1, 1, 0.4f));
            yield return new WaitForSeconds(duration / 4.0f);

            animController.SetMaterialColor(new Color(1, 1, 1, 1f));
            yield return new WaitForSeconds(duration / 4.0f);
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

    public void ChangeFallState()
    {
        isFall = true;
        stateMachine.ChangeState(fallState);
    }

    public bool isTutorial = false;
    public bool isClearTutorial = false;
    public void InitbySceneLoaded(SceneInfo curScene)
    {
        isBounded = false;
        if(curScene == SceneInfo.Start)
        {
            SetIdleStatePlayer();
            isStateChangeable = false;
            return;
        }
        
        Vector3 pos = GameObject.FindGameObjectWithTag("StartPos").transform.position;
        
        switch (curScene)
        {
            case SceneInfo.Town_1:      // 1
                isCombatZone = false;
                if (isTutorial)
                {
                    isTutorial = false;
                    pos = new Vector3(-2.5f, 22.5f, 0);
                }
                break;
            case SceneInfo.Tutorial:    // 2
                isTutorial = true;
                break;
            case SceneInfo.Puzzle_1:    // 3
            case SceneInfo.Battle_1_A:  // 4
            case SceneInfo.Battle_1_B:  // 5
            case SceneInfo.Battle_1_C:  // 6
            case SceneInfo.Boss_1:      // 7
                isAttackable = true;
                isCombatZone = true;
                break;
        }

        ControlPlayerPos(pos);
    }

    public void ControlPlayerPos(Vector3 pos)
    {
        transform.position = pos;
    }

    public void ChangePlayerInteractionState(bool state)
    {
        isInteraction = state;   // player의 상호작용 여부 관찰
        if (state) SetIdleStatePlayer();

        isStateChangeable = !state;
        isAttackable = !state;
    }
}
