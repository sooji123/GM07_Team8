using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

[Serializable]
public class PoolData
{
    public string objName;
    public GameObject prefab;
    public int count;
}

public class PoolManager : Singleton<PoolManager>
{
    [SerializeField]
    private PoolData[] _poolData = null;

    private Dictionary<string, IObjectPool<GameObject>> ojbectPoolDic = new Dictionary<string, IObjectPool<GameObject>>();

    protected override void Awake()
    {
        base.Awake();
        Init();
    }
    private void Init()
    {
        if (_poolData == null) 
        {
            return;
        }

        for(int i=0;i< _poolData.Length; i++)
        {
            CreatePool(_poolData[i]);
        }
    }

    public void CreatePool(PoolData info)
    {
        if (info == null || info.prefab == null || info.objName == null) return;

        if (ojbectPoolDic.ContainsKey(info.objName))
        {
            return;
        }
        IObjectPool<GameObject> pool = null;
        pool = new ObjectPool<GameObject>(
            createFunc: () => {
                GameObject poolGo = Instantiate(info.prefab, transform);

                if (!poolGo.TryGetComponent<PoolAble>(out var poolAble))
                {
                    poolAble = poolGo.AddComponent<PoolAble>();
                }
                poolAble.Pool = pool;
                return poolGo;
            },
            actionOnGet: OnTakeFromPool,
            actionOnRelease: OnReturnedToPool,
            actionOnDestroy: OnDestroyPoolObject,
            collectionCheck: true,
            defaultCapacity: info.count,
            maxSize: info.count * 2
        );

        ojbectPoolDic.Add(info.objName, pool);

        List<GameObject> gameObjects = new List<GameObject>();
        for (int j = 0; j < info.count; j++)
        {
            gameObjects.Add(pool.Get());
        }
        foreach (var obj in gameObjects)
        {
            pool.Release(obj);
        }
    }

    private void OnTakeFromPool(GameObject poolGo)
    {
        poolGo.SetActive(true);
    }

    private void OnReturnedToPool(GameObject poolGo)
    {
        poolGo.SetActive(false);
    }

    private void OnDestroyPoolObject(GameObject poolGo)
    {
        Destroy(poolGo);
    }

    public GameObject GetGo(string goName)
    {
        if (ojbectPoolDic.TryGetValue(goName, out IObjectPool<GameObject> pool))
        {
            return pool.Get();
        }

        Debug.LogError($"{goName} 오브젝트풀에 등록되지 않은 이름입니다.");
        return null;
    }
}
public class PoolAble : MonoBehaviour
{
    public IObjectPool<GameObject> Pool { get; set; }

    public void ReleaseObject()
    {
        Pool.Release(gameObject);
    }
}
