using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [System.Serializable]
    public struct WaveData
    {
        public string waveName;
        public GameObject enemyPrefab;
        public int count;
        public float spawnDelay;
    }

    [Header("--- 캐릭터 배치 ---")]
    public GameObject playerPrefab;
    public Transform playerSpawnPoint;

    [Header("--- 웨이브 경로 지정 ---")]
    public Transform spawnPoint;
    public Transform[] route;

    [Header("--- 웨이브 리스트 설정 ---")]
    public List<WaveData> waves = new List<WaveData>();

    private Animator playerAnimator;
    private int currentWaveIndex = 0;

    private List<GameObject> aliveEnemies = new List<GameObject>();

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

        for (int i = 0; i < wave.count; i++)
        {
            GameObject newEnemy = EnemyObjectPool.Instance.SpawnFromPool(wave.enemyPrefab, spawnPoint.position, Quaternion.identity);

            EnemyBase moveScript = newEnemy.GetComponent<EnemyBase>();
            if (moveScript != null)
            {
                moveScript.wayPoints = route;
                moveScript.waveManager = this;
            }

            aliveEnemies.Add(newEnemy);

            yield return new WaitForSeconds(wave.spawnDelay);
        }

        if (playerAnimator != null)
        {
            playerAnimator.SetInteger("SpellAction", 0);
        }

        isSpawning = false;
        Debug.Log($"{wave.waveName} 모든 몹 소환 완료!");
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
        }
    }
}