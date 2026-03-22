using UnityEngine;

public class BearDetectRange : MonoBehaviour
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
            bearAI.playerInDetectRange = true;
            bearAI.target = other.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            bearAI.playerInDetectRange = false;

            // Nếu ra khỏi detect thì chắc chắn cũng không còn attack
            bearAI.playerInAttackRange = false;
            bearAI.target = null;
        }
    }
}