using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private bool isActivated = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isActivated) return;

        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return;

        Vector3 checkpointPos = transform.position + new Vector3(0f, 1f, 0f);
        player.SetRespawnPoint(checkpointPos);

        Debug.Log("Checkpoint activated at: " + checkpointPos);
        isActivated = true;
    }
}