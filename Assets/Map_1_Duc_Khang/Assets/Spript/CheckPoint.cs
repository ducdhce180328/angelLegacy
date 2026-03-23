using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private bool isActivated = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isActivated) return;
        if (!other.CompareTag("Player")) return;

        PlayerController player = other.GetComponent<PlayerController>();

        Vector3 checkpointPos = transform.position + new Vector3(0f, 1f, 0f);
        if (player != null)
        {
            player.SetRespawnPoint(checkpointPos);
        }

        CheckpointManager.SetCheckpoint(checkpointPos);

        Debug.Log("Checkpoint activated at: " + checkpointPos);
        isActivated = true;
    }
}
