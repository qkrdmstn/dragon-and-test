using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapOutOfRange : MonoBehaviour
{
    [SerializeField] SceneInfo myScene;
    [SerializeField] int _goToScene;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (gameObject.name.Contains("Battle"))
            {
                //_sceneInfo = Random.Range(4, 6);
                _goToScene = 4;
            }

            UIManager.instance.StartFade(_goToScene);

            if (myScene == SceneInfo.Tutorial && _goToScene == (int)SceneInfo.Town_1)
            {
                GameManager.instance.isTutorial = true;
            }
        }
    }
}
