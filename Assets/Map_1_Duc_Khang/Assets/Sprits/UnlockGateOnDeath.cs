using UnityEngine;

public class UnlockGateOnDeath : MonoBehaviour
{
    public GameObject gateToOpen;
    private EnemyHealth enemyHealth;
    private bool opened = false;

    private void Awake()
    {
        enemyHealth = GetComponent<EnemyHealth>();
    }

    private void Update()
    {
        if (opened) return;
        if (enemyHealth == null) return;

        if (enemyHealth.currentHealth <= 0)
        {
            opened = true;

            if (gateToOpen != null)
            {
                gateToOpen.SetActive(false);
            }
        }
    }
}