using TMPro;
using UnityEngine;

public class TutorialUIGroup : UIGroup
{
    Animator tutoUIAnim;
    TutorialInteraction tutorial;

    public GameObject[] dialogUIs;
    public TextMeshProUGUI[] dialogTxts;
    [SerializeField] Animator nextKeyAnim;

    void Awake()
    {
        tutoUIAnim = GetComponent<Animator>();
    }

    void Update()
    {
        if (ScenesManager.instance.GetSceneEnum() != SceneInfo.Tutorial || ScenesManager.instance.isLoading) return;

        if(tutorial == null)
            tutorial = GameObject.FindFirstObjectByType<TutorialInteraction>();
    }

    public void SetNextKey(string name)
    {
        nextKeyAnim.SetTrigger(name);
    }

    public override void SwitchAnim(string animName, bool state)
    {   // tutorial - isInteraction, isAttack, isDash, isReload, isOpenJokbo
        // blanket - isSkillInfo, isHwatuCombination, isSkillKey, isTrash
        tutoUIAnim.SetBool(animName, state);
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
