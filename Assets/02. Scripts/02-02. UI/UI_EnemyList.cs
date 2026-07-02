using UnityEngine;

public class UI_EnemyList : MonoBehaviour
{
    [Header("WaveManager 연결")]
    [SerializeField]
    private WaveManager waveManager;

    [SerializeField]
    private UI_EnemySlot slotPrefab;
    [SerializeField]
    private Transform slotParent;

    WaveManager.WaveData[] waves;

    //초기화 및 설정
    //가능하면 instantiate/destroy를 최소화 하는 pooling 사용 고려
    public void RefreshEnemyList()
    {
        foreach (Transform child in slotParent)
        {
            Destroy(child.gameObject);
        }

        WaveManager.WaveData wave = GetCurrentWave();

        foreach (WaveManager.SpawnGroup group in wave.spawnGroups)
        {
            UI_EnemySlot slot = Instantiate(slotPrefab, slotParent);

            Sprite sprite = group.enemyPrefab.GetComponent<SpriteRenderer>().sprite;

            slot.Setup(sprite, group.groupName, group.isBarrier, group.isRegen, group.count);
        }
    }

    public WaveManager.WaveData GetCurrentWave()
    {
        if (waveManager.CurrentWaveIndex <= 0)
        {
            return default;
        }
        return waves[waveManager.CurrentWaveIndex - 1];
    }
}
