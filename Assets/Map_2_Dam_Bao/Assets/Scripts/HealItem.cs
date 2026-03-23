using UnityEngine;

public class HealItem : MonoBehaviour
{
    public int healAmount = 20;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (PlayerCompatibilityUtility.TryHeal(other, healAmount))
        {
            Destroy(gameObject);
        }
    }
}
