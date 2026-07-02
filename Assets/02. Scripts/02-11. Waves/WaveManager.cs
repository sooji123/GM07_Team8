using System.Collections;
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

        [Header("--- 기믹 ---")]
        [Tooltip("Barrier")] public bool isBarrier;
        [Tooltip("Regen")] public bool isRegen;
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
            UI_Manager.Instance.GameClearWindow();
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
                        group.isBarrier, group.isRegen
                    ));
                }
                else
                {
                    normalEnemies.Add(new SpawnGroupInfo(
                        group.enemyPrefab, group.spawnDelay,
                        group.isBarrier, group.isRegen
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
                fixedEnemy.isBarrier, fixedEnemy.isRegen
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
            if (enemyInfo.prefab == null)
            {
                Debug.LogError($"[웨이브 매니저 에러] 웨이브 데이터 설정 중 'Enemy Prefab' 칸이 비어있습니다! 인스펙터를 확인해 주세요. (해당 몹 스킵)");
                yield return new WaitForSeconds(enemyInfo.spawnDelay);
                continue;
            }

            if (EnemyObjectPool.Instance == null)
            {
                Debug.LogError($"[웨이브 매니저 에러] 메인 씬에 'EnemyObjectPool' 오브젝트(혹은 싱글톤 인스턴스)가 존재하지 않습니다! 스폰을 중단합니다.");
                isSpawning = false;
                yield break;
            }

            GameObject newEnemy = EnemyObjectPool.Instance.SpawnFromPool(enemyInfo.prefab, spawnPoint.position, Quaternion.identity);

            if (newEnemy == null)
            {
                Debug.LogError($"[웨이브 매니저 에러] 오브젝트 풀에서 프리팹 '{enemyInfo.prefab.name}'을(를) 꺼내지 못했습니다. 풀의 등록 리스트나 최대 수량을 확인하세요. (해당 몹 스킵)");
                yield return new WaitForSeconds(enemyInfo.spawnDelay);
                continue;
            }

            EnemyBase moveScript = newEnemy.GetComponent<EnemyBase>();
            if (moveScript != null)
            {
                moveScript.wayPoints = route;
                moveScript.waveManager = this;

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
        if (aliveEnemies == null) return;
        if (aliveEnemies.Contains(enemy))
        {
            aliveEnemies.Remove(enemy);
        }
        if (!isSpawning && aliveEnemies.Count == 0)
        {
            Debug.Log("웨이브 클리어!");
            startButton.WaveEnded();
        }
        if (!isSpawning &&aliveEnemies.Count == 0 && currentWaveIndex >= waves.Count)
        {
            Debug.Log("모든 웨이브 클리어!");
            UI_Manager.Instance.GameClearWindow();
        }
    }

    private struct SpawnGroupInfo
    {
        public GameObject prefab;
        public float spawnDelay;
        public bool isBarrier;
        public bool isRegen;

        public SpawnGroupInfo(GameObject prefab, float spawnDelay, bool isBarrier, bool isRegen)
        {
            this.prefab = prefab;
            this.spawnDelay = spawnDelay;
            this.isBarrier = isBarrier;
            this.isRegen = isRegen;
        }
    }

    private struct FixedSpawnInfo
    {
        public GameObject prefab;
        public float spawnDelay;
        public int targetIndex;
        public bool isBarrier;
        public bool isRegen;

        public FixedSpawnInfo(GameObject prefab, float spawnDelay, int targetIndex, bool isBarrier, bool isRegen)
        {
            this.prefab = prefab;
            this.spawnDelay = spawnDelay;
            this.targetIndex = targetIndex;
            this.isBarrier = isBarrier;
            this.isRegen = isRegen;
        }
    }

    //테스트용 Wave Skip 기능 추가(테스트 종료시 제거 자유)
    public void OnClickSkipWave()
    {
        if (isSpawning || aliveEnemies.Count > 0)
        {
            Debug.Log($"아직 이전 웨이브가 끝나지 않았습니다! (남은 몹: {aliveEnemies.Count}마리)");
            return;
        }

        if (currentWaveIndex < waves.Count)
        {
            currentWaveIndex++;
            Debug.Log($"{currentWaveIndex}번 Wave 스킵");
        }
        else
        {
            Debug.Log("모든 웨이브가 끝났습니다!");
            UI_Manager.Instance.GameClearWindow();
        }
    }
}