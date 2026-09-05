using UnityEngine;

// 障碍使用实体碰撞；藤蔓头本身的触发器负责生长阶段的接触检测。
public class ObstacleDamage : MonoBehaviour
{
    [SerializeField] private gameManager manager;
    [SerializeField] private int damage = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        CheckVine(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        CheckVine(other);
    }

    private void CheckVine(Collider2D other)
    {
        if (Time.timeScale <= 0f || !manager.IsGrowing)
            return;

        VineEnd head = other.GetComponent<VineEnd>();
        if (head != null && head.manager == manager)
            manager.GrowthFailed();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        DamagePlayer(collision.collider);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        DamagePlayer(collision.collider);
    }

    private void DamagePlayer(Collider2D other)
    {
        if (Time.timeScale <= 0f || manager.IsGrowing)
            return;

        Health health = other.GetComponentInParent<Health>();
        if (health != null && health.CompareTag("Player"))
            health.TakeDamage(damage);
    }
}
