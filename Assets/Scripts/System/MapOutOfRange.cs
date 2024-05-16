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
            if (gameObject.name.Contains("Battle"))
            {
                //_sceneInfo = Random.Range(4, 6);
                _sceneInfo = 4;
            }
            UIManager.instance.fade.ManageFade(_sceneInfo);

            if (_sceneInfo < 5)
                SoundManager.instance.ManageSound(_sceneInfo);
            else StartCoroutine(SoundManager.instance.FadeOutSound());
        }
    }
}
