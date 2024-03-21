using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    #region Components
    private Player player;
    private SpriteRenderer renderer;
    #endregion

    [Header("Gun Timer")]
    public float shootTimer;

    [Header("Gun information")]
    public int damage = 1;
    [SerializeField] private float shootDelay;
    [SerializeField] private float reloadTime;
    [SerializeField] private bool isReloading = false;
    [SerializeField] private bool clickReloadFlag = false;

    //public int maxBullet; //�ִ� �Ѿ� ����
    //public int curTotalBullet; //���� ���� �Ѿ�
    [SerializeField] private int magazineSize; //źâ ũ��
    [SerializeField] private int loadedBullet; //���� ������ �Ѿ�

    [Header("Bullet Prefabs")]
    public GameObject bulletPrefab;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        renderer = gameObject.GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        shootTimer -= Time.deltaTime;

        if (player.isAttackable && !isReloading)
        {
            if (Input.GetKeyUp(KeyCode.Mouse0) && loadedBullet == 0)
                clickReloadFlag = true;

            if (clickReloadFlag && Input.GetKeyDown(KeyCode.Mouse0))
                Reload();
            else if (Input.GetKey(KeyCode.Mouse0))
                Shoot();

            if (Input.GetKeyDown(KeyCode.R))
                Reload();
        }
        else if (!player.isAttackable && player.stateMachine.currentState == player.dashState)
        {
            if (!isReloading)
            {
                if (Input.GetKeyDown(KeyCode.R))
                    Reload();
            }
        }
    }

    public void Shoot()
    {
        if(loadedBullet > 0 && shootTimer < 0.0)
        {
            shootTimer = shootDelay;
            loadedBullet--;

            //Create Bullet
            GameObject bulletObj = Instantiate(bulletPrefab, transform.position, transform.rotation);
            Rigidbody2D rigid = bulletObj.GetComponent<Rigidbody2D>();
            Bullet bullet = bulletObj.GetComponent<Bullet>();

            Vector2 dir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            dir.Normalize();

            bullet.BulletInitialize(damage, dir);
        }
    }
    
    public void Reload()
    {
        if(loadedBullet != magazineSize)
        {
            //Debug.Log("Reload Start");
            clickReloadFlag = false;
            isReloading = true;

            //Reload Visulaize
            renderer.color = new Color(1, 0, 0);
            StartCoroutine(ReloadProcess());
        }
    }

    IEnumerator ReloadProcess()
    {
        yield return new WaitForSeconds(reloadTime);
        loadedBullet = magazineSize;
        isReloading = false;
        //Debug.Log("Reload End");

                    
        renderer.color = new Color(1, 1, 1);
    }
}
