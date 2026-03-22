using UnityEngine;

public class BearAttackRange : MonoBehaviour
{
    public BearAI bearAI;

    private void Awake()
    {
        if (bearAI == null)
            bearAI = GetComponentInParent<BearAI>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            bearAI.playerInAttackRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            bearAI.playerInAttackRange = false;
        }
    }
}