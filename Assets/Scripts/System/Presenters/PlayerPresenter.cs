using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class PlayerPresenter : PresenterBase
{
    [Header("Model")]
    public Player m_Player;

    [Header("View")]
    public Sprite[] HPSprites;
    public Image HPImage;

    public Sprite[] ShieldSprites;
    public Image ShieldImage;

    public TextMeshProUGUI moneyText;

    private void Start()
    {
        m_Player.actions[(int)StatActionType.HP] += HealthChanged;
        m_Player.actions[(int)StatActionType.Money] += MoneyChanged;
        m_Player.actions[(int)StatActionType.Shield] += ShieldChanged;
    }

    private void OnDestroy()
    {
        m_Player.actions[(int)StatActionType.HP] -= HealthChanged;
        m_Player.actions[(int)StatActionType.Money] -= MoneyChanged;
        m_Player.actions[(int)StatActionType.Shield] -= ShieldChanged;
    }

    #region HP
    public void Heal(int amount)
    {
        m_Player.IncrementHP(amount);
    }

    public void Damage(int amount)
    {
        m_Player.DecrementHP(amount);
    }

    public void ResetHP()
    {
        m_Player.RestoreHP();
    }
    public void HealthChanged()
    {
        UpdateHPView();
    }

    void UpdateHPView()
    {
        if (Player.instance.refCurHp == 0)
        {
            HPImage.sprite = null;
            HPImage.color = Color.clear;
        }
        else
        {
            HPImage.sprite = HPSprites[m_Player.refCurHp];
            HPImage.color = Color.white;
        }
    }
    #endregion

    #region SHIELD
    public void AddShield(int amount)
    { 
        m_Player.ReplaceShield(amount);
    }
    public void ShieldChanged()
    {
        UpdateShieldView();
    }

    void UpdateShieldView()
    {
        if (m_Player.refShield == 0)
        {
            ShieldImage.sprite = null;
            ShieldImage.color = Color.clear;
        }
        else
        {
            ShieldImage.sprite = ShieldSprites[m_Player.refShield];
            ShieldImage.color = Color.white;
        }
    }
    #endregion

    #region MONEY
    public void AddMoney(int amount)
    {
        m_Player.IncrementMoney(amount);
    }

    public void SubMoney(int amount)
    {
        m_Player.DecrementMoney(amount);
    }

    public void MoneyChanged()
    {
        UpdateMoneyView();
    }

    void UpdateMoneyView()
    {
        moneyText.text = "X " + m_Player.refMoney;
    }
    #endregion

    public override SceneInfo ActivateEachUI()
    {
        if (base.ActivateEachUI() == SceneInfo.Battle_1_A)
        {   // UI기 켜져야하는 배틀, 튜토리얼, 보스, 퍼즐씬
            objs[0].SetActive(true);    // hp & shield
            objs[1].SetActive(true);    // money
        }
        return SceneInfo.Battle_1_A;
    }
}
