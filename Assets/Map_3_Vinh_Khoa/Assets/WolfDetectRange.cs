using UnityEngine;

public class WolfDetectRange : MonoBehaviour
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
            wolfAI.playerInDetectRange = true;
            wolfAI.target = other.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            wolfAI.playerInDetectRange = false;
            wolfAI.playerInAttackRange = false;
            wolfAI.target = null;
        }
    }
}