using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUIGroup : UIGroup
{
    public bool isWASD, isAttack, isDash, isSkill, isReload; // UI가 뜨고 플레이어가 해당 UI에 대한 key를 누르면 활성화
    public Vector3 padding;
    Animator anim;

    private void Start()
    {
        padding = new Vector3(0, 175f, 0);
        anim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        anim.enabled = true;
    }

    private void Update()
    {
        transform.position = Camera.main.WorldToScreenPoint(UIManager.instance.player.transform.position) + padding;

        if (!isWASD && UIManager.instance.fade.fadePanel.color.a < 0.1) anim.SetBool("isWASD", true);
    }

    private void OnDisable()
    {
        isWASD = false;
        isAttack = false;
        isDash  = false;
        isSkill = false;
        isReload = false;

        anim.Rebind();
        anim.enabled = false;
    }

    IEnumerator CheckStateCouroutine(string curType)
    {   // UI 활성화 애니메이션이 끝나면 호출되는 코루틴, UI 비활성화 ON
        yield return new WaitUntil(() => CheckState(curType));

        if (curType == "Reload" && isReload)
        {
            Debug.Log("reload-false");
            anim.SetBool("isReload", false);
        }
        else if (curType == "Skill" && isSkill)
        {
            anim.SetBool("isSkill", false);
            anim.SetBool("isReload", true);
        }
        else if (curType == "Move" && isWASD)
        {
            anim.SetBool("isWASD", false);
            anim.SetBool("isSkill", true);
        }
    }

    public bool CheckState(string curType)
    {   // 현재 진행 UI에 따라서 다음 애니메이션을 준비
        if (!isWASD && curType == "Move" && (Input.GetKeyDown(KeyCode.W)
            || Input.GetKeyDown(KeyCode.A)
            || Input.GetKeyDown(KeyCode.S)
            || Input.GetKeyDown(KeyCode.D)))
        {
            isWASD = true;
            return isWASD;
        }

        else if (isWASD && !isAttack && Input.GetKeyDown(KeyCode.Mouse0))
        {
            isAttack = true;
            Debug.Log("isAttack");
        }

        else if (isWASD && !isDash && Input.GetKeyDown(KeyCode.Mouse1) && UIManager.instance.player.IsDash())
        {
            isDash = true;
            Debug.Log("isDash");
        }

        else if (isAttack && isDash && curType == "Skill")
        {   // 위의 조건에 플레이어가 움직이는 상태가 추가되어야	함
            isSkill = true;
            Debug.Log("isSkill");
            return isSkill;
        }

        else if (!isReload && curType == "Reload" && Input.GetKeyDown(KeyCode.R))
        {
            isReload = true;
            Debug.Log("isReload");
            return isReload;
        }

        return false;
    } 
}
