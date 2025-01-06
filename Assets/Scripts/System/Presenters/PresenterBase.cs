using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PresenterBase : MonoBehaviour
{
    public GameObject[] objs;
    public virtual bool ActivateEachUI()
    {
        if (ScenesManager.instance.GetSceneEnum() == SceneInfo.Start || ScenesManager.instance.GetSceneEnum() == SceneInfo.Town_1)
        {
            foreach (GameObject ui in objs)
            {
                ui.SetActive(false);
            }
            return true;
        }
        else
        {   // tutorial, battle, puzzle, boss
            return false;
        }
    }
}
