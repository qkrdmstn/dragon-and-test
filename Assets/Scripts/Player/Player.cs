using System.Collections;
using UnityEngine;
using Cinemachine;
using System;
using System.Collections.Generic;

public enum StatActionType
{
    HP, Money, Shield
}

public class Player : MonoBehaviour
{
    #region Value
    public static Player instance = null;
    public Action[] actions;

    [Header("Life info")]
    int curHP = 10;
    int maxHP = 10;
    public int refCurHp
    {
        get { return curHP; }
        set { 
            curHP = Mathf.Clamp(value, 0, maxHP);
            actions[(int)StatActionType.HP].Invoke();
        } 
    }

    int money = 0;
    public int refMoney
    {
        get { return money; }
        set { 
            money = Mathf.Clamp(value, 0, value);
            actions[(int)StatActionType.Money].Invoke();
        }
    }

    int shield = 0;
    int maxShield = 3;
    public int refShield
    {
        get { return shield; }
        set
        {
            shield = Mathf.Clamp(value, 0, maxShield);
            actions[(int)StatActionType.Shield].Invoke();
            // if value is zero, Update Inventory UI
            if (shield <= 0) ItemManager.instance.UpdateArmorData();
        }
    }
    [SerializeField] private float hitDuration = 0.6f;

    [Header("Move info")]
    public float moveSpeed = 5.65f;
    public float dashSpeed = 13.0f;
    public float dashDuration = 0.58f;
    public float dashExpCoefficient = -3.5f;

    [Header("Position info")]
    public Queue<Vector2> positionHistoryQueue = new Queue<Vector2>();
    public int positionHistoryQueueSize;
    public float positionSaveInterval;
    public float positionSaveTimer;

    [Header("Knockback info")]
    public Vector2 knockbackDir;
    public float knockbackMagnitude;
    public float knockbackExpCoefficient = -0.1f;

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
    public bool isTownStart = false;
    public bool isSuperman = false;
    public bool isCursorStart = true;

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
        actions = new Action[Enum.GetValues(typeof(StatActionType)).Length];

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
    }

    public void IncrementHP(int amount) => refCurHp += amount;

    public void DecrementHP(int amount) => refCurHp -= amount;
    public void RestoreHP() => refCurHp = maxHP;

    public void ReplaceShield(int amount) => refShield = amount;

    public void DecrementShield(int amount) => refShield -= amount;

    public void IncrementMoney(int amount) => refMoney += amount;

    public void DecrementMoney(int amount) => refMoney -= amount;

    public void SetVelocity(float _xVelocity, float _yVelocity)
    {
        rb.velocity = new Vector2(_xVelocity, _yVelocity);
    }

    public void SetVelocity(Vector2 vel)
    {
        rb.velocity = vel;
    }

    #region FUNC_GET

    public Vector2 GetVelocity() => rb.velocity;
    public int GetCurHP() => refCurHp;
    public int GetMaxHP() => maxHP ;
    public int GetCurShield() => refShield;
    public int GetCurMoney() => refMoney;
    #endregion

    public float ClacSpeed(float baseSpeed)
    {
        float speed = baseSpeed;
        if (SkillManager.instance.PassiveCheck(SeotdaHwatuCombination.SR46))
            speed += baseSpeed * SkillManager.instance.GetSkillProb(SeotdaHwatuCombination.SR46);
        if (isSuperman)
            speed += baseSpeed * SkillManager.instance.GetSkillProb(SeotdaHwatuCombination.GTT38);

        return speed;
    }

    public void OnDamaged(int damage)
    {
        if(IsDash())
        {  
            SkillManager.instance.DashCoolTimeAdvantage();
            return;
        }    

        if(!isDamaged)
        {
            try
            {    // monster에게 맞았을때 쉐이킹
                cameraManager.CameraShakeFromProfile(profile, impulseSource);
            }
            catch (System.NullReferenceException ex)
            {
                cameraManager = FindObjectOfType<CameraManager>();
                cameraManager.CameraShakeFromProfile(profile, impulseSource);
            }
            if (isTutorial)
                return;

            isDamaged = true;
            if (refShield > 0)
                DecrementShield(damage);
            else
                DecrementHP(damage);

            if (refCurHp <= 0) //Dead
            {
                //장사
                //4% 확률로 죽음 회피 & 체력 회복
                SkillDB js410Data = SkillManager.instance.GetSkillDB(SeotdaHwatuCombination.JS410);
                float js410Prob = SkillManager.instance.GetSkillProb(SeotdaHwatuCombination.JS410);
                float randomVal = UnityEngine.Random.Range(0.0f, 1.0f);
                if (SkillManager.instance.PassiveCheck(SeotdaHwatuCombination.JS410) && randomVal <= js410Prob)
                {
                    refCurHp = 1;
                    isDamaged = false;
                }
                else
                {
                    PlayerDead();
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

    private void PlayerDead()
    {
        isDead = true;
        GameManager.instance.SetTimeScale(0f);
        UIManager.instance.SceneUI["Dead"].SetActive(true);
    }

    public void ReloadPlayer()
    {   
        isAttackable = false;
        isCombatZone = false;
        isDead = false;
        isDamaged = false;

        RestoreHP();
        refMoney = 0;
        refShield = 0; // inventory UI도 같이 갱신

        SkillManager.instance.ClearSkill(); // 모든 스킬 삭제
        ItemManager.instance.DeleteAllHwatuCards(); // 모든 화투 삭제
        ItemManager.instance.gunController.ClearGunDatas(); // 모든 총 초기화

        animController.isBreath = false;
        animController.SetAnim(PlayerAnimState.Idle);
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
                InitPositionHistoryQueue();
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

    public void InitPositionHistoryQueue()
    {
        positionHistoryQueue.Clear();
    }

    public void PositionHistorySave()
    {
        if (isCombatZone && !isFall)
        {
            if (positionHistoryQueue.Count >= positionHistoryQueueSize) //오래된 위치값 제거
                positionHistoryQueue.Dequeue();

            //현재 위치 저장(블록 내의 격자 값)
            positionHistoryQueue.Enqueue(this.transform.position);

            //Queue Draw
            Vector2[] arr = positionHistoryQueue.ToArray();
            for (int i = 0; i < arr.Length; i++)
            {
                Vector2 recoveryPosition = arr[i];
                Vector3 v1 = recoveryPosition + new Vector2(-1.0f, -1.0f) / 2.0f;
                Vector3 v2 = recoveryPosition + new Vector2(1.0f, -1.0f) / 2.0f;
                Vector3 v3 = recoveryPosition + new Vector2(-1.0f, 1.0f) / 2.0f;
                Vector3 v4 = recoveryPosition + new Vector2(1.0f, 1.0f) / 2.0f;
                Debug.DrawLine(v1, v2, Color.green, 1.0f);
                Debug.DrawLine(v1, v3, Color.green, 1.0f);
                Debug.DrawLine(v2, v4, Color.green, 1.0f);
                Debug.DrawLine(v3, v4, Color.green, 1.0f);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("BlanketMap"))
        {   // player가 모포가 있는 맵에 들어왔으며, 그 맵의 모포가 아직 유효하다면(destroy되지 않음) 신비로운 효과음 플레이
            BlockInfo mapBlock = collision.GetComponentInParent<BlockInfo>();
            BlanketInteractionData data = mapBlock.GetComponentInChildren<BlanketInteractionData>();
            if (data!=null) 
                SoundManager.instance.SetEffectSound(SoundType.UI, UISfx.GoBlanketMap);
        }
    }
}
