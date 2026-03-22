using UnityEngine;

public class SkillLProjectile : MonoBehaviour
{
    private float direction;
    private int damage;
    private float speed;

    public float lifeTime = 2f;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Init(float dir, int dmg, float moveSpeed)
    {
        direction = dir;
        damage = dmg;
        speed = moveSpeed;

        if (rb != null)
        {
            rb.linearVelocity = new Vector2(direction * speed, 0f);
        }

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * direction;
        transform.localScale = scale;

        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyHealth enemy = collision.GetComponent<EnemyHealth>();

        if (enemy == null)
        {
            enemy = collision.GetComponentInParent<EnemyHealth>();
        }

        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        if (!collision.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}