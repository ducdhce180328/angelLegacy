using UnityEngine;

public class WolfAttackRange : MonoBehaviour
{
    public WolfAI wolfAI;

    private void Awake()
    {
        if (wolfAI == null)
            wolfAI = GetComponentInParent<WolfAI>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            wolfAI.playerInAttackRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            wolfAI.playerInAttackRange = false;
        }
    }
}