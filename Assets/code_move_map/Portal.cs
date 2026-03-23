using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    [Header("Target Scene")]
    public string targetSceneName;

    [Header("Spawn In New Scene")]
    public Vector3 targetSpawnPosition;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerSpawnData.spawnPosition = targetSpawnPosition;
        PlayerSpawnData.hasSpawnPosition = true;

        SceneManager.LoadScene(targetSceneName);
    }
}