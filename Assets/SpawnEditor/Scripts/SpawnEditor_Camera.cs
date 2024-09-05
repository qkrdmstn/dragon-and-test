using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEditor_Camera : MonoBehaviour
{
    //Input
    [Header("Input info")]
    [SerializeField] private float xInput;
    [SerializeField] private float yInput;
    [SerializeField] private float scroll;

    [Header("Move info")]
    [SerializeField] private float vel = 5f;
    [SerializeField] private float zoomVel = 1.5f;

    // Start is called before the first frame update
    void Start()
    {
        CameraSetting();
    }

    // Update is called once per frame
    void Update()
    {
        CameraControl();
    }

    public void CameraSetting()
    {
        if (SpawnEditorManager.instance.curState == SpawnEditor_State.BlockSetting)
        {
            Vector2 centerPos = SpawnEditorManager.instance.totalBlock.blockCenterPos;
            this.transform.position = new Vector3(centerPos.x, centerPos.y, this.transform.position.z);
            Camera.main.orthographicSize = 65.0f;
        }
        else if (SpawnEditorManager.instance.curState == SpawnEditor_State.WaveSetting)
        {
            Vector2 centerPos = SpawnEditorManager.instance.selectedBlock.blockCenterPos;
            this.transform.position = new Vector3(centerPos.x, centerPos.y, this.transform.position.z);
            Camera.main.orthographicSize = 14.0f;
        }
    }

    private void CameraControl()
    {
        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");
        scroll = Input.GetAxisRaw("Mouse ScrollWheel");

        //Camera Movement
        float nextX = this.transform.position.x + xInput * vel * Camera.main.orthographicSize * Time.deltaTime;
        float nextY = this.transform.position.y + yInput * vel * Camera.main.orthographicSize * Time.deltaTime;
        float nextZ = this.transform.position.z;
        Vector3 nextPos = new Vector3(nextX, nextY, nextZ);
        if (SpawnEditorManager.instance.curState == SpawnEditor_State.BlockSetting)
        {
            if (SpawnEditorManager.instance.totalBlock.IsInBlock(nextPos))
                this.transform.position = nextPos;


        }
        else if (SpawnEditorManager.instance.curState == SpawnEditor_State.WaveSetting)
        {
            if (SpawnEditorManager.instance.selectedBlock.IsInBlock(nextPos))
                this.transform.position = nextPos;
        }

        //Zoom In/Out
        if (scroll > 0.0f)
            Camera.main.orthographicSize -= scroll * Camera.main.orthographicSize;
        else if (scroll < 0.0f)
            Camera.main.orthographicSize -= scroll * Camera.main.orthographicSize;
    }
}
