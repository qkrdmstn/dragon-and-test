using UnityEngine;
using UnityEngine.Pool;

public class MonsterPool : MonoBehaviour
{

    public static MonsterPool instance;

    public int defaultCapacity = 30;
    public int maxPoolSize = 30;
    public GameObject monsterBullet;

    public IObjectPool<GameObject> pool { get; private set; }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);


        Init();
    }

    private void Init()
    {
        pool = new ObjectPool<GameObject>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool,
        OnDestroyPoolObject, true, defaultCapacity, maxPoolSize);

        // 미리 오브젝트 생성 해놓기
        for (int i = 0; i < defaultCapacity; i++)
        {
            MonsterBullet bullet = CreatePooledItem().GetComponent<MonsterBullet>();
            bullet.pool.Release(bullet.gameObject);
        }
    }

    // 생성
    private GameObject CreatePooledItem()
    {
        GameObject poolGo = Instantiate(monsterBullet, this.transform);
        poolGo.GetComponent<MonsterBullet>().pool = this.pool;
        return poolGo;
    }

    // 사용
    private void OnTakeFromPool(GameObject poolGo)
    {
        poolGo.SetActive(true);
    }

    // 반환
    private void OnReturnedToPool(GameObject poolGo)
    {
        poolGo.SetActive(false);
    }

    // 삭제
    private void OnDestroyPoolObject(GameObject poolGo)
    {
        Destroy(poolGo);
    }
}