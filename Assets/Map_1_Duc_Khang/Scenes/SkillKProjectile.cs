using UnityEngine;

public class SkillKProjectile : MonoBehaviour
{
    public float speed = 8f;
    public float lifeTime = 2f;
    public int damage = 20;

    private float direction = 1f;
    private bool hasHit = false;

    public void Init(float newDirection, int newDamage, float newSpeed)
    {
        direction = Mathf.Sign(newDirection);
        damage = newDamage;
        speed = newSpeed;

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * direction;
        transform.localScale = scale;
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        transform.Translate(Vector2.right * direction * speed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return;

        if (other.CompareTag("Player")) return;

        EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();

        if (enemyHealth == null)
        {
            enemyHealth = other.GetComponentInParent<EnemyHealth>();
        }

        if (enemyHealth != null)
        {
            hasHit = true;
            enemyHealth.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        // đụng vật cản khác thì tự hủy
        if (!other.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}