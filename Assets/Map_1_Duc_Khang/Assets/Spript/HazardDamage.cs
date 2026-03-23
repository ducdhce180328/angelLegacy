using UnityEngine;

public class HazardDamage : MonoBehaviour
{
    public int damage = 10;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerCompatibilityUtility.TryTakeDamage(other, damage);
        }
    }
}
