using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapOutOfRange : MonoBehaviour
{
    public bool isTrigger = false;
    [SerializeField] SceneInfo _goToScene;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isTrigger)
        {
            if (_goToScene == SceneInfo.Battle_1_A)
            {
                _goToScene = (SceneInfo)Random.Range(5, 8);
            }
            else if(_goToScene == SceneInfo.Puzzle_1)
            {   
                if (!transform.GetComponentInParent<BlockInfo>().blockClear) return;
            }
            isTrigger = true;
            ScenesManager.instance.isLoading = true;
            Player.instance.SetIdleStatePlayer();
            Player.instance.isStateChangeable = false;

            UIManager.instance.StartFade((int)_goToScene);
        }
    }
}
