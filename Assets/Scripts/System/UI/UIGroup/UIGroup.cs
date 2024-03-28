using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGroup : MonoBehaviour
{
    public GameObject[] childUI;

    public virtual void ToggleUI(GameObject _ui)
    {
        if (_ui.activeSelf)
            _ui.SetActive(false);
        else
            _ui.SetActive(true);
    }
}
