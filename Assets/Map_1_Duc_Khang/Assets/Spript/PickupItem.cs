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

        isPicked = true;

        if (pickupType == PickupType.Health)
        {
            if (!PlayerCompatibilityUtility.TryHeal(other, amount))
            {
                isPicked = false;
                return;
            }
        }
        else if (pickupType == PickupType.Mana)
        {
            if (!PlayerCompatibilityUtility.TryRestoreMana(other, amount))
            {
                isPicked = false;
                return;
            }
        }

        Destroy(gameObject);
    }
}
