using UnityEngine;

public class TestMissile : MonoBehaviour
{
    private float damage;
    private EElement element;
    private Transform target;

    [Header("--- 테스트 총알 설정 ---")]
    public float speed = 7f;

    public void Initialize(float damage, EElement element, Transform target)
    {
        this.damage = damage;
        this.element = element;
        this.target = target;
    }

    void Update()
    {
        if (target == null || !target.gameObject.activeSelf)
        {
            Destroy(gameObject);
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, target.position) < 0.1f)
        {
            HitTarget();
        }
    }

    void HitTarget()
    {
        EnemyBase enemy = target.GetComponent<EnemyBase>();

        if (enemy != null)
        {
            enemy.TakeDamage(damage, element);
        }

        Destroy(gameObject);
    }
}