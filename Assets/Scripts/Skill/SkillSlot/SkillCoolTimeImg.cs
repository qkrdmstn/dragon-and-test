using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillCoolTimeImg : MonoBehaviour
{
    public Image img;
    // Start is called before the first frame update
    void Awake()
    {
        img = GetComponent<Image>(); 
    }


}
