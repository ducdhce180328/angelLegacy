using UnityEngine;

public class ManaItem : MonoBehaviour
{
    public int manaAmount = 20;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (PlayerCompatibilityUtility.TryRestoreMana(other, manaAmount))
        {
            Destroy(gameObject);
        }
    }
}
