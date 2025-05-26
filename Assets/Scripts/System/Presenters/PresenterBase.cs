using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PresenterBase : MonoBehaviour
{
    public GameObject[] objs;
    //public virtual bool ActivateEachUI()
    //{
    //    if (ScenesManager.instance.GetSceneEnum() == SceneInfo.Start || ScenesManager.instance.GetSceneEnum() == SceneInfo.Town_1)
    //    {
    //        foreach (GameObject ui in objs)
    //        {
    //            ui.SetActive(false);
    //        }
    //        return true;
    //    }
    //    else
    //    {   // tutorial, battle, puzzle, boss
    //        return false;
    //    }
    //}

    public virtual SceneInfo ActivateEachUI()
    {
        if (ScenesManager.instance.GetSceneEnum() == SceneInfo.Start)
        {
            foreach (GameObject ui in objs)
            {
                ui.SetActive(false);
            }
            return SceneInfo.Start;
        }
        else if (ScenesManager.instance.GetSceneEnum() == SceneInfo.Town_1)
        {   // for inventory 
            return SceneInfo.Town_1;
        }
        else if (ScenesManager.instance.GetSceneEnum() == SceneInfo.StartCutScene)
        {
            return SceneInfo.StartCutScene;
        }
        else
        {   // tutorial, battle, puzzle, boss
            return SceneInfo.Battle_1_A;
        }
    }
}
