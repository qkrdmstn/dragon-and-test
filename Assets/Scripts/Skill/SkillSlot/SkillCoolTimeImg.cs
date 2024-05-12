using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillCoolTimeImg : MonoBehaviour
{
    Image coolTimeImg;

    // Start is called before the first frame update
    void Start()
    {
        coolTimeImg = GetComponent<Image>(); 
    }

    public void SetCoolTimeUI(float coolTime)
    {
        StartCoroutine(CoolTimeFunc(coolTime));
    }

    IEnumerator CoolTimeFunc(float coolTime)
    {
        float timer = coolTime;
        while (timer > 0.0f)
        {
            timer -= Time.deltaTime;
            coolTimeImg.fillAmount = timer / coolTime;
            yield return new WaitForFixedUpdate();
        }
        gameObject.SetActive(false);
    }
}
