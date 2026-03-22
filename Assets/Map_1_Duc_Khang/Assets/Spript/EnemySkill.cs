using UnityEngine;

public class EnemySkill : MonoBehaviour
{
    public float skillRange = 1.5f;
    public float cooldown = 1.5f;

    private Transform player;
    private float timer;
    private EnemyHealth enemyHealth;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        enemyHealth = GetComponent<EnemyHealth>();
    }

    private void Update()
    {
        if (player == null || enemyHealth == null) return;

        timer += Time.deltaTime;

        if (Vector2.Distance(transform.position, player.position) <= skillRange)
        {
            if (timer >= cooldown)
            {
                UseSkill();
                timer = 0f;
            }
        }
    }

    void UseSkill()
    {
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(enemyHealth.damageToPlayer);
        }
    }
}