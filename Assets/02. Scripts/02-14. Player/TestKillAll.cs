using UnityEngine;

public class TestKillAll : MonoBehaviour
{
    public void OnClickKillAllThem()
    {
        EnemyBase[] enemyInScreen = FindObjectsByType<EnemyBase>(FindObjectsSortMode.None);

        foreach (EnemyBase enemy in enemyInScreen)
        {
            float damage = 100.0f;
            enemy.TakePercentageDamage(damage);
        }
    }
}
