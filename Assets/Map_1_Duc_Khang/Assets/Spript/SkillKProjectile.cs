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

        Component receiver = EnemyCompatibilityUtility.GetDamageReceiver(other);
        if (receiver != null)
        {
            hasHit = true;
            EnemyCompatibilityUtility.ApplyDamageWithKnockback(receiver, damage, transform.position);
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
