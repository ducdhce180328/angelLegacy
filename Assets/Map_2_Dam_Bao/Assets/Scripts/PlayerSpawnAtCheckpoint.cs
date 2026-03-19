using UnityEngine;

public class PlayerSpawnAtCheckpoint : MonoBehaviour
{
    void Start()
    {
        if (CheckpointManager.hasCheckpoint)
        {
            transform.position = CheckpointManager.respawnPosition;
        }
    }
}