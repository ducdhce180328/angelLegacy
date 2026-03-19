using UnityEngine;

public class ManaItem : MonoBehaviour
{
    public int manaAmount = 20;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        Mana playerMana = other.GetComponentInParent<Mana>();
        Health playerHealth = other.GetComponentInParent<Health>();

        if (playerMana != null && playerHealth != null && !playerHealth.isDead)
        {
            if (playerMana.currentMana < playerMana.maxMana)
            {
                playerMana.RestoreMana(manaAmount);
                Destroy(gameObject);
            }
        }
    }
}