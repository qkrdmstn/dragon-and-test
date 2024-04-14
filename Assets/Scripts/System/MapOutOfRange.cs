using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapOutOfRange : MonoBehaviour
{
    [SerializeField] int _sceneInfo;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            UIManager.instance.fade.ManageFade(_sceneInfo);

            if (_sceneInfo < 3)
                SoundManager.instance.ManageSound(_sceneInfo);
            else StartCoroutine(SoundManager.instance.FadeOutSound());
        }
    }
}
