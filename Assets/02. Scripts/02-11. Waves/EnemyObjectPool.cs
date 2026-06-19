using System.Collections.Generic;
using UnityEngine;

public class EnemyObjectPool : Singleton<EnemyObjectPool>
{
    [System.Serializable]
    public struct PoolInfo
    {
        public GameObject prefab;
        public int size;
    }

    public List<PoolInfo> pools;

    private Dictionary<GameObject, Queue<GameObject>> poolDictionary = new Dictionary<GameObject, Queue<GameObject>>();

    void Start()
    {
        InitializePools();
    }

    void InitializePools()
    {
        foreach (var pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab, transform);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.prefab, objectPool);
        }
    }

    [Header("--- 이펙트 설정 ---")]
    public string effectTag = "EnemyHitEffect";
    public float defaultEffectDuration = 0.3f;

    public GameObject SpawnFromPool(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(prefab))
        {
            Debug.LogWarning($"[EnemyObjectPool] {prefab.name}에 대한 풀이 존재하지 않습니다.");
            return null;
        }

        GameObject objectToSpawn = poolDictionary[prefab].Dequeue();

        if (objectToSpawn.activeSelf)
        {
            objectToSpawn = Instantiate(prefab, transform);
        }

        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        objectToSpawn.SetActive(true);

        poolDictionary[prefab].Enqueue(objectToSpawn);

        if (objectToSpawn.CompareTag(effectTag))
        {
            StartCoroutine(DisableEffectRoutine(objectToSpawn, defaultEffectDuration));
        }

        return objectToSpawn;
    }

    private System.Collections.IEnumerator DisableEffectRoutine(GameObject effectObj, float duration)
    {
        yield return new WaitForSeconds(duration);

        if (effectObj != null && effectObj.activeSelf)
        {
            effectObj.SetActive(false);
        }
    }
}