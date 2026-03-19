using UnityEngine;

public static class CheckpointManager
{
    public static Vector3 respawnPosition;
    public static bool hasCheckpoint = false;

    public static void SetCheckpoint(Vector3 position)
    {
        respawnPosition = position;
        hasCheckpoint = true;
    }

    public static void ResetCheckpoint()
    {
        hasCheckpoint = false;
        respawnPosition = Vector3.zero;
    }
}