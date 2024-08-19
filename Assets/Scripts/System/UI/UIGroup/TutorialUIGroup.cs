using UnityEngine;

public class TutorialUIGroup : UIGroup
{
    Animator anim;
    Tutorial tutorial;
    [SerializeField] Vector3 padding;
    [SerializeField] float adjustUIPosition;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (ScenesManager.instance.GetSceneEnum() != SceneInfo.Tutorial || ScenesManager.instance.isLoading) return;

        if(tutorial == null)
        {
            tutorial = GameObject.FindFirstObjectByType<Tutorial>();
        }

        GameObject curScarescrow = tutorial.GetCurScarescrow();
        if (curScarescrow && attachUIIdx > -1)
        {   // 플레이어와 허수아비 위치에 따라 키 UI의 위치 조정
            if (curScarescrow.transform.position.y + adjustUIPosition < Player.instance.transform.position.y) // 허수아비보다 플레이어가 위에 있다면,
                childUI[attachUIIdx].transform.position = Camera.main.WorldToScreenPoint(Player.instance.transform.position) + padding;
            else
                childUI[attachUIIdx].transform.position = Camera.main.WorldToScreenPoint(Player.instance.transform.position) + -padding;
        }
  
    }

    public override void SwitchAnim(string animName, bool state)
    {
        anim.SetBool(animName, state);
    }

    int attachUIIdx = -1;
    public override void AttachUIforPlayer(int childUIIdx)
    {
        attachUIIdx = childUIIdx;
    }
}
