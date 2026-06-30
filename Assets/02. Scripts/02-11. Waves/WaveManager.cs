using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WaveManager : MonoBehaviour
{
    [System.Serializable]
    public struct SpawnGroup
    {
        public string groupName;
        public GameObject enemyPrefab;
        public int count;
        public float spawnDelay;

        [Header("--- 고정 등장 설정 ---")]
        public bool isFixedPosition;
        public int fixedIndex;

        [Header("--- 몹 기믹 ---")]
        [Tooltip("Shield")] public bool isShield;
        [Tooltip("Barrier")] public bool isBarrier;
        [Tooltip("Regeneration")] public bool isRegen;
    }

    [System.Serializable]
    public struct WaveData
    {
        public string waveName;
        public bool shuffleSpawnOrder;
        public List<SpawnGroup> spawnGroups;
    }

    [Header("--- 캐릭터 배치 ---")]
    public GameObject playerPrefab;
    public Transform playerSpawnPoint;

    [Header("--- 웨이브 경로 지정 ---")]
    public Transform spawnPoint;
    public Transform[] route;

    [Header("--- 웨이브 리스트 설정 ---")]
    public List<WaveData> waves = new List<WaveData>();

    [Header("--- Start Button 연결 ---")]
    public StartButtonUI startButton;

    private Animator playerAnimator;
    private int currentWaveIndex = 0;

    public int CurrentWaveIndex => currentWaveIndex;

    private List<GameObject> aliveEnemies = new List<GameObject>();

    public List<GameObject> _aliveEnemies => aliveEnemies;

    private bool isSpawning = false;

    void Start()
    {
        SpawnPlayer();
    }

    public void StartNextWave()
    {
        if (isSpawning || aliveEnemies.Count > 0)
        {
            Debug.Log($"아직 이전 웨이브가 끝나지 않았습니다! (남은 몹: {aliveEnemies.Count}마리)");
            return;
        }

        if (currentWaveIndex < waves.Count)
        {
            StartCoroutine(SpawnWaveRoutine(waves[currentWaveIndex]));
            currentWaveIndex++;
        }
        else
        {
            Debug.Log("모든 웨이브가 끝났습니다!");
        }
    }

    void SpawnPlayer()
    {
        if (playerPrefab != null && playerSpawnPoint != null)
        {
            GameObject player = Instantiate(playerPrefab, playerSpawnPoint.position, playerSpawnPoint.rotation);
            playerAnimator = player.GetComponent<Animator>();
        }
    }

    IEnumerator SpawnWaveRoutine(WaveData wave)
    {
        Debug.Log($"{wave.waveName} 시작!");
        isSpawning = true;

        if (playerAnimator != null)
        {
            playerAnimator.SetInteger("SpellAction", 1);
        }

        List<SpawnGroupInfo> normalEnemies = new List<SpawnGroupInfo>();
        List<FixedSpawnInfo> fixedEnemies = new List<FixedSpawnInfo>();

        foreach (SpawnGroup group in wave.spawnGroups)
        {
            for (int i = 0; i < group.count; i++)
            {
                if (group.isFixedPosition)
                {
                    fixedEnemies.Add(new FixedSpawnInfo(
                        group.enemyPrefab, group.spawnDelay, group.fixedIndex,
                        group.isShield, group.isBarrier, group.isRegen
                    ));
                }
                else
                {
                    normalEnemies.Add(new SpawnGroupInfo(
                        group.enemyPrefab, group.spawnDelay,
                        group.isShield, group.isBarrier, group.isRegen
                    ));
                }
            }
        }

        if (wave.shuffleSpawnOrder)
        {
            ShuffleList(normalEnemies);
        }

        fixedEnemies.Sort((a, b) => a.targetIndex.CompareTo(b.targetIndex));

        foreach (FixedSpawnInfo fixedEnemy in fixedEnemies)
        {
            int insertIndex = fixedEnemy.targetIndex - 1;

            SpawnGroupInfo mappedInfo = new SpawnGroupInfo(
                fixedEnemy.prefab, fixedEnemy.spawnDelay,
                fixedEnemy.isShield, fixedEnemy.isBarrier, fixedEnemy.isRegen
            );

            if (insertIndex >= normalEnemies.Count)
            {
                normalEnemies.Add(mappedInfo);
            }
            else if (insertIndex < 0)
            {
                normalEnemies.Insert(0, mappedInfo);
            }
            else
            {
                normalEnemies.Insert(insertIndex, mappedInfo);
            }
        }

        foreach (SpawnGroupInfo enemyInfo in normalEnemies)
        {
            GameObject newEnemy = EnemyObjectPool.Instance.SpawnFromPool(enemyInfo.prefab, spawnPoint.position, Quaternion.identity);

            EnemyBase moveScript = newEnemy.GetComponent<EnemyBase>();
            if (moveScript != null)
            {
                moveScript.wayPoints = route;
                moveScript.waveManager = this;

                moveScript.useShieldBlock = enemyInfo.isShield;
                moveScript.useMagicBarrier = enemyInfo.isBarrier;
                moveScript.useRegeneration = enemyInfo.isRegen;
                moveScript.RefreshGimmickVisual();
            }

            aliveEnemies.Add(newEnemy);

            yield return new WaitForSeconds(enemyInfo.spawnDelay);
        }

        if (playerAnimator != null)
        {
            playerAnimator.SetInteger("SpellAction", 0);
        }

        isSpawning = false;
        Debug.Log($"{wave.waveName} 모든 몹 소환 완료!");
    }

    void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int rnd = Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[rnd];
            list[rnd] = temp;
        }
    }

    public void RemoveEnemy(GameObject enemy)
    {
        if (aliveEnemies.Contains(enemy))
        {
            aliveEnemies.Remove(enemy);
        }

        if (!isSpawning && aliveEnemies.Count == 0)
        {
            Debug.Log("웨이브 클리어!");
            startButton.WaveEnded();
        }
    }

    private struct SpawnGroupInfo
    {
        public GameObject prefab;
        public float spawnDelay;
        public bool isShield;
        public bool isBarrier;
        public bool isRegen;

        public SpawnGroupInfo(GameObject prefab, float spawnDelay, bool isShield, bool isBarrier, bool isRegen)
        {
            this.prefab = prefab;
            this.spawnDelay = spawnDelay;
            this.isShield = isShield;
            this.isBarrier = isBarrier;
            this.isRegen = isRegen;
        }
    }

    private struct FixedSpawnInfo
    {
        public GameObject prefab;
        public float spawnDelay;
        public int targetIndex;
        public bool isShield;
        public bool isBarrier;
        public bool isRegen;

        public FixedSpawnInfo(GameObject prefab, float spawnDelay, int targetIndex, bool isShield, bool isBarrier, bool isRegen)
        {
            this.prefab = prefab;
            this.spawnDelay = spawnDelay;
            this.targetIndex = targetIndex;
            this.isShield = isShield;
            this.isBarrier = isBarrier;
            this.isRegen = isRegen;
        }
    }
}