using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public enum StateOfBuy
{   // 0 : 의사결정중 1:구매 2:미구매 3:구매불가
    Nothing,
    YesBuy,
    NoBuy,
    CantBuy
};

public class ShopInteraction : Interaction
{
    #region Item UI
    Image itemImg;
    TextMeshProUGUI itemName, itemInfo, itemPrice;
    #endregion

    #region Dialog UI
    TextMeshProUGUI dialogueTxt;
    List<Image> selectionImg;
    #endregion

    public StateOfBuy state;
    public bool isFirst = true;
    public bool isSelected = false;
    public int result; // 선택된 답변의 배열 Idx
    public bool isFirstBuyWeapon = true;

    GameObject[] childUI;
    GameObject interaction;
    ItemData itemData;
    ShopUIGroup shopUIGroup;

    public override void LoadEvent(InteractionData data)
    {
        Init();

        this.itemData = data.itemData;
        interaction = data.gameObject;

        if (isFirst) SetShopUI();

        SetActiveSelectUI(true);
        SetActiveShopUI(true);

        UpdateItemData();
        UpdateDialogTxt(state);

        StartCoroutine(ManageEvent());
    }

    void SetShopUI()
    {   // 관련 변수 할당
        shopUIGroup = UIManager.instance.SceneUI["Shop"].GetComponent<ShopUIGroup>();
        childUI = shopUIGroup.childUI;

        // item
        itemImg = childUI[0].GetComponent<Image>();
        itemName = childUI[1].GetComponent<TextMeshProUGUI>();
        itemInfo = childUI[2].GetComponent<TextMeshProUGUI>();
        itemPrice = childUI[3].GetComponent<TextMeshProUGUI>();

        // dialog
        dialogueTxt = childUI[5].GetComponent<TextMeshProUGUI>();
        selectionImg = new List<Image>
        {
            childUI[6].GetComponent<Image>(),
            childUI[7].GetComponent<Image>()
        };

        isFirst = false;
    }

    void UpdateDialogTxt(StateOfBuy curCase)
    {   // 대화 string 등록
        string curLine = "";

        switch (curCase)
        {
            case StateOfBuy.Nothing:
                curLine = "가격은 " + itemData.price+"금이다.\r\n구매할 것이냐?";
                break;
            case StateOfBuy.YesBuy:
                if(itemData.itemType == ItemType.Gun && isFirstBuyWeapon)
                {
                    curLine = "구매한 무기는 <b>마우스 휠</b>을 변경하여\n사용할 수 있으니 참고하도록...";
                    isFirstBuyWeapon = false;
                }
                else curLine = "고맙다.";
                break;
                case StateOfBuy.NoBuy:
                curLine = "다른 것도 천천히 둘러보도록...";
                break;
            case StateOfBuy.CantBuy:
                curLine = "돈이 부족한 것 같군..\n돈을 더 벌어오도록...";
                break;
        }

        dialogueTxt.text = curLine;
    }

    void UpdateItemData()
    {
        itemImg.sprite = itemData.icon;
        itemName.text = itemData.itemName;
        itemInfo.text = itemData.itemInfo;
        itemPrice.text = itemData.price.ToString();
    }

   bool UpdateDialogue()
    {   // manage Dialogue event
        if (shopUIGroup.isExit || Input.GetKeyDown(KeyCode.Escape))
            isDone = true;     // 대화 도중 나갈 수 있습니다.
        else if (isSelected && Input.GetKeyDown(KeyCode.F))
            isDone = true;

        if (isDone) SetActiveShopUI(false);

        // 선택 대화 출력
        if (result == -1) Selection(2);  // 처음 선택에 대한 디폴트 선택을 지정합니다. 이후 방향키 입력이 있다면, 발동되지 않습니다

        if (Input.GetKeyDown(KeyCode.W))
        {
            result = 0;
            Selection(0);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            result = 1;
            Selection(1);
        }
        else if (Input.GetKeyDown(KeyCode.F) && result > -1)
        {   // 특정 경우를 선택한 경우
            isSelected = true;
            SetActiveSelectUI(false);
            UpdateDialogTxt(CheckBuyState(result));
        }
        return isDone;
    }

    public StateOfBuy CheckBuyState(int result)
    {
        if(result == 1) state = StateOfBuy.NoBuy;

        else if(Player.instance.refMoney < itemData.price)
        {   // checkMoney
            state = StateOfBuy.CantBuy;
        }
        else
        {
            state = StateOfBuy.YesBuy;
            ManagePurchase();
        }
        return state;
    }

    void ManagePurchase()
    {   // 구매시,
        switch (itemData.itemType)
        {
            case ItemType.Material:
                if ((itemData as EffectItemData) != null)
                    (itemData as EffectItemData).ItemEffect();
                break;
            case ItemType.Gun:
                GunManager.instance.AddGun(itemData as GunItemData);
                break;
            case ItemType.Armor:
                (itemData as EffectItemData).ItemEffect();
                InventoryData.instance.AddArmorItem(itemData);
                break;
        }
        Player.instance.refMoney -= itemData.price;
        Destroy(interaction);
    }

    IEnumerator ManageEvent()
    {
        yield return new WaitUntil(() => !isFirst);

        yield return new WaitUntil(() => UpdateDialogue());    // 의사결정완료

        SetActiveShopUI(false);
        isDone = false;
    }

    void Init()
    {
        result = -1;
        isSelected = false;
        state = StateOfBuy.Nothing;

        interaction = null;
        itemData = null;
    }

    Color selectedColor = new Color(0.75f, 0.92f, 0.88f);
    void Selection(int idx)
    {   // input = 선택한 분기의 인덱스 : w - 0 / s - 1
        selectionImg[(idx + 1) % 2].color = Color.white;    // 반투명
        selectionImg[idx % 2].color = selectedColor;

        if (idx == 2)   // defalut Selection
            result = 0;
    }

    void SetActiveShopUI(bool visible)
    {   // manage dialog UI
        UIManager.instance.SceneUI["Inventory"].SetActive(!visible);    // inventory UI

        if (visible) UIManager.instance.PushPopUI(shopUIGroup.gameObject);
        else UIManager.instance.isClose = true;
    }

    void SetActiveSelectUI(bool visible)
    {   // manage select UI
        selectionImg[0].gameObject.SetActive(visible);
        selectionImg[1].gameObject.SetActive(visible);
    }
}
