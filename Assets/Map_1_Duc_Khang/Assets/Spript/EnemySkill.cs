using UnityEngine;

public class EnemySkill : MonoBehaviour
{
    private const float PlayerRefreshInterval = 0.5f;

    public float skillRange = 1.5f;
    public float cooldown = 1.5f;

    private Transform player;
    private float timer;
    private EnemyHealth enemyHealth;
    private float playerRefreshTimer;

    private void Start()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        RefreshPlayerReference(true);
    }

    private void Update()
    {
        RefreshPlayerReference();

        if (player == null || enemyHealth == null || enemyHealth.isDead) return;

        if (enemyHealth.IsHitStunned)
        {
            timer = 0f;
            return;
        }

        timer += Time.deltaTime;

        if (Vector2.Distance(transform.position, player.position) <= skillRange && timer >= cooldown)
        {
            UseSkill();
            timer = 0f;
        }
    }

    void UseSkill()
    {
        if (player == null || enemyHealth == null || enemyHealth.IsHitStunned || enemyHealth.isDead) return;

        PlayerCompatibilityUtility.TryTakeDamage(player, enemyHealth.damageToPlayer);
    }

    private void RefreshPlayerReference(bool force = false)
    {
        if (!force && player != null && player.gameObject.activeInHierarchy && !PlayerCompatibilityUtility.IsDead(player.gameObject))
        {
            return;
        }

        if (!force)
        {
            playerRefreshTimer -= Time.deltaTime;
            if (playerRefreshTimer > 0f)
            {
                return;
            }
        }

        playerRefreshTimer = PlayerRefreshInterval;

        GameObject playerObj = PlayerCompatibilityUtility.FindPlayer();
        player = playerObj != null ? playerObj.transform : null;
    }
}
