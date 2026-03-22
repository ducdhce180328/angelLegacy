using UnityEngine;

public class WizardProjectile : MonoBehaviour
{
    [Header("Hit Settings")]
    [SerializeField] private LayerMask hitObstacleLayer;

    private Vector2 moveDirection;
    private float moveSpeed;
    private float maxDistance;
    private int damage;

    private Vector3 startPosition;
    private bool isInitialized;

    private PlayerStats ownerStats;

    public void Initialize(Vector2 direction, float speed, float distance, int projectileDamage, PlayerStats stats)
    {
        moveDirection = direction.normalized;
        moveSpeed = speed;
        maxDistance = distance;
        damage = projectileDamage;
        ownerStats = stats;

        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        startPosition = transform.position;
        isInitialized = true;
    }

    private void Update()
    {
        if (!isInitialized)
            return;

        transform.position += (Vector3)(moveDirection * moveSpeed * Time.deltaTime);

        float traveledDistance = Vector3.Distance(startPosition, transform.position);
        if (traveledDistance >= maxDistance)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyHealth enemyHealth = collision.GetComponentInParent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damage);

            if (ownerStats != null)
            {
                ownerStats.HealOnHit();
            }

            Destroy(gameObject);
            return;
        }

        if (((1 << collision.gameObject.layer) & hitObstacleLayer) != 0)
        {
            Destroy(gameObject);
        }
    }
}