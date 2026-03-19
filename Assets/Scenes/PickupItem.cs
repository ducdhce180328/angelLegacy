using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public enum PickupType
    {
        Health,
        Mana
    }

    [Header("Pickup")]
    public PickupType pickupType;
    public int amount = 10;

    private bool isPicked = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isPicked) return;
        if (!other.CompareTag("Player")) return;

        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

        if (playerHealth == null)
        {
            playerHealth = other.GetComponentInParent<PlayerHealth>();
        }

        if (playerHealth == null) return;

        isPicked = true;

        if (pickupType == PickupType.Health)
        {
            playerHealth.AddHealth(amount);
        }
        else if (pickupType == PickupType.Mana)
        {
            playerHealth.AddMana(amount);
        }

        Destroy(gameObject);
    }
}