using TMPro;
using UnityEngine;

public class TutorialUIGroup : UIGroup
{
    Animator tutoUIAnim;
    TutorialInteraction tutorial;

    public GameObject[] dialogUIs;
    public TextMeshProUGUI[] dialogTxts;

    [SerializeField] Animator nextKeyAnim;
    [SerializeField] Vector3 padding;
    [SerializeField] float adjustUIPosition;

    private void Awake()
    {
        tutoUIAnim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (ScenesManager.instance.GetSceneEnum() != SceneInfo.Tutorial || ScenesManager.instance.isLoading) return;

        if(tutorial == null)
        {
            tutorial = GameObject.FindFirstObjectByType<TutorialInteraction>();
        }

        GameObject curScarescrow = tutorial.GetCurScarescrowObj();
        if (curScarescrow && attachUIIdx > -1)
        {   // 플레이어와 허수아비 위치에 따라 키 UI의 위치 조정
            if (curScarescrow.transform.position.y + adjustUIPosition < Player.instance.transform.position.y) // 허수아비보다 플레이어가 위에 있다면,
                childUI[attachUIIdx].transform.position = Camera.main.WorldToScreenPoint(Player.instance.transform.position) + padding;
            else
                childUI[attachUIIdx].transform.position = Camera.main.WorldToScreenPoint(Player.instance.transform.position) + -padding;
        }
    }

    public void SetNextKey(string name)
    {
        nextKeyAnim.SetTrigger(name);
    }

    public override void SwitchAnim(string animName, bool state)
    {
        tutoUIAnim.SetBool(animName, state);
    }

    int attachUIIdx = -1;
    public override void AttachUIforPlayer(int childUIIdx)
    {
        attachUIIdx = childUIIdx;
    }

    public void InactiveAllDialogUIs()
    {
        foreach(GameObject obj in dialogUIs)
        {
            obj.SetActive(false);
        }
    }

    public void ResetDialogTxts()
    {
        foreach(TextMeshProUGUI textMeshProUGUI in dialogTxts)
        {
            textMeshProUGUI.text = "";
        }
    }
}
