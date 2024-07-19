using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorControl : MonoBehaviour
{
    [SerializeField] Sprite cursorImg;

    public Camera cam;
    public Transform player;

    [Header("Threshold")]
    [Tooltip("지정한 거리만큼까지만 cursor의 이동 위치를 카메라가 follow합니다")]
    [SerializeField] float cursor_threshold;

    [Tooltip("지정한 거리만큼 player에게서 cursor가 멀어지면 카메라가 cursor를 follow합니다")]
    [SerializeField] float player_threshold;

    Vector3 worldPos, targetPos;

    private void Start()
    {
        cursor_threshold = 3f;
        player_threshold = 3;
        SetCursor(cursorImg);
    }

    private void Update()
    {
        if (ScenesManager.instance.GetSceneNum() < 1) return;
        player = Player.instance.transform;

        worldPos = cam.ScreenToWorldPoint(Input.mousePosition);
        targetPos = (player.position + worldPos) / 2f;

        if (Diff_FromPlayerToCursor(player.position, worldPos))
        {
            targetPos.x = Mathf.Clamp(targetPos.x, -cursor_threshold + player.position.x, cursor_threshold + player.position.x);
            targetPos.y = Mathf.Clamp(targetPos.y, -cursor_threshold + player.position.y, cursor_threshold + player.position.y);

            transform.position = targetPos;
        }
        else transform.position = player.position;
    }

    public void SetCursor(Sprite _cursorImg)
    {
        Texture2D cursorTex;
        Vector2 cursorPoint;

        cursorTex = UIManager.instance.TextureFromSprite(_cursorImg);
        cursorPoint = new Vector2(cursorTex.width / 2, cursorTex.height / 2);
        Cursor.SetCursor(cursorTex, cursorPoint, CursorMode.ForceSoftware);
        Cursor.lockState = CursorLockMode.Confined; // 게임 창 밖으로 커서가 나가지 못함
    }

    bool Diff_FromPlayerToCursor(Vector3 player, Vector3 cursor)
    {   // player와 cursor사이의 거리를 구합니다
        float diff_x = Mathf.Pow(cursor.x - player.x, 2);
        float diff_y = Mathf.Pow(cursor.y - player.y, 2);
        float diff = Mathf.Sqrt(diff_x + diff_y);

        return (diff > player_threshold) ? true : false;
    }
}   
