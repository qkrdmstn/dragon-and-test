using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUIGroup : UIGroup
{
    public bool isWASD, isAttack, isDash, isSkill, isReload; // UI가 뜨고 플레이어가 해당 UI에 대한 key를 누르면 활성화
    public Vector3 padding;
    Animator anim;
    Player player;

    private void OnEnable()
    {
        padding = new Vector3(0, 175f, 0);
        anim = GetComponent<Animator>();
        player = FindObjectOfType<Player>();
    }

    private void Update()
    {
        transform.position = Camera.main.WorldToScreenPoint(player.transform.position) + padding;

        if (!isWASD && UIManager.instance.fade.fadePanel.color.a < 0.1) anim.SetBool("isWASD", true);
        else if (isSkill) { anim.SetBool("isReload", true); }
        else if (isWASD) { anim.SetBool("isSkill", true); }
    }

    IEnumerator CheckStateCouroutine(string curType)
    {   // UI 활성화 애니메이션이 끝나면 호출되는 코루틴, UI 비활성화 ON
        yield return new WaitUntil(() => CheckState(curType));

        if(curType == "Move" && isWASD)
        {
            anim.SetBool("isWASDOut", true);
        }
        else if (curType == "Skill" && isSkill)
        {
            anim.SetBool("isSkillOut", true);
        }
        else if (curType == "Reload" && isReload)
        {
            anim.SetBool("isReloadOut", true);
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

        else if (isWASD && !isDash && Input.GetKeyDown(KeyCode.Mouse1))
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
