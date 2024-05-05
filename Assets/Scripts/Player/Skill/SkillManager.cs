using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager instance = null;

    public List<Hwatu> hwatuCardSlotData = new List<Hwatu>();

    private void Awake()
    {
        if (instance == null)
        { //생성 전이면
            instance = this; //생성
        }
        else if (instance != this)
        { //이미 생성되어 있으면
            if (ScenesManager.instance.GetSceneNum() == (int)SceneInfo.Battle_1
                || ScenesManager.instance.GetSceneNum() == (int)SceneInfo.Puzzle_1
                || ScenesManager.instance.GetSceneNum() == (int)SceneInfo.Boss_1)
            {
                Destroy(instance); //새로만든거 삭제
            }
            Destroy(this.gameObject); //새로만든거 삭제
        }

        DontDestroyOnLoad(this.gameObject); //씬이 넘어가도 오브젝트 유지
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab) && !GameManager.instance.player.isInteraction) //Skill Slot Swap
        {
            Hwatu temp = hwatuCardSlotData[0];
            hwatuCardSlotData[0] = hwatuCardSlotData[1];
            hwatuCardSlotData[1] = temp;

            // Todo Skill Slot Update
        }
    }

    public void AddSkill(Hwatu hwatu1)
    {
        if(hwatuCardSlotData.Count < 2)
        {
            hwatuCardSlotData.Add(hwatu1);

        }
        else
        {
            Debug.Log("Select Delete Card");
        }

        //SeotdaHwatuCombination result = Hwatu.GetHwatuCombination(hwatu1);
        //Debug.Log(result);
    }
}
