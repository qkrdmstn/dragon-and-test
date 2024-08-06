using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapOutOfRange : MonoBehaviour
{
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
            ScenesManager.instance.isLoading = true;
            UIManager.instance.StartFade(_goToScene);
        }
    }
}
