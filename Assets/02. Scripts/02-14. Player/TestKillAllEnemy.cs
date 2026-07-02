using UnityEngine;

public class TestKillAllEnemy : MonoBehaviour
{
    public void OnClickKillAllEnemy()
    {
        EnemyBase[] enemyInScreen = FindObjectsByType<EnemyBase>(FindObjectsSortMode.None);

        foreach (EnemyBase enemy in enemyInScreen)
        {
            float damage = 100.0f;
            enemy.TakePercentageDamage(damage);
        }
    }
}
