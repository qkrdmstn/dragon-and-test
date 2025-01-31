using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Effect", menuName = "Data/ItemEffect")]
public class ItemEffect : ScriptableObject
{
    protected PlayerPresenter playerPresenter;

    void NullCheckPresernter()
    {
        if (playerPresenter == null) playerPresenter = UIManager.instance.presenters[(int)PresenterType.Player] as PlayerPresenter;
    }
    public virtual void ExcuteEffect()
    {
        NullCheckPresernter();
        Debug.Log("Excute Effect");
    }

    public virtual void ExcuteEffect(int amount)
    {
        NullCheckPresernter();
        Debug.Log("Excute Effect - amount "+ amount);
    }
}
