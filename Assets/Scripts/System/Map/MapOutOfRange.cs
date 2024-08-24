using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapOutOfRange : MonoBehaviour
{
    public bool isTrigger = false;
    [SerializeField] int _goToScene;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isTrigger)
        {
            isTrigger = true;
            if (gameObject.name.Contains("Battle"))
            {
                _goToScene = Random.Range(4, 7);
            }
            else if(_goToScene == 3)
            {
                if (!transform.GetComponentInParent<BlockInfo>().blockClear) return;
            }
            ScenesManager.instance.isLoading = true;
            Player.instance.SetIdleStatePlayer();
            Player.instance.isStateChangeable = false;

            UIManager.instance.StartFade(_goToScene);
        }
    }
}
