using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public Transform spawnPoint;

    private void Start()
    {
        SpawnSelectedCharacter();
    }

    void SpawnSelectedCharacter()
    {
        if (CharacterSelectionManager.Instance == null)
        {
            Debug.LogWarning("Khong tim thay CharacterSelectionManager");
            return;
        }

        GameObject selectedPrefab = CharacterSelectionManager.Instance.selectedCharacterPrefab;

        if (selectedPrefab == null)
        {
            Debug.LogWarning("Chua co nhan vat duoc chon");
            return;
        }

        if (spawnPoint == null)
        {
            Debug.LogWarning("Chua gan Spawn Point");
            return;
        }

        GameObject spawnedPlayer = Instantiate(selectedPrefab, spawnPoint.position, Quaternion.identity);
        EnsurePlayerCompatibility(spawnedPlayer);
    }

    void EnsurePlayerCompatibility(GameObject player)
    {
        if (player == null) return;

        PlayerMovementController modernMovement = player.GetComponent<PlayerMovementController>();
        PlayerController legacyController = player.GetComponent<PlayerController>();
        PlayerHealth legacyHealth = player.GetComponent<PlayerHealth>();

        if (modernMovement != null && legacyController != null)
        {
            legacyController.enabled = false;
        }

        if (legacyController != null && legacyHealth != null)
        {
            if (player.GetComponent<LegacyPlayerStats>() == null && player.GetComponent<PlayerStats>() == null)
            {
                player.AddComponent<LegacyPlayerStats>();
            }

            if (player.GetComponent<PlayerUpgradeInput>() == null)
            {
                player.AddComponent<PlayerUpgradeInput>();
            }

            if (player.GetComponent<PlayerSpeechBubble>() == null)
            {
                player.AddComponent<PlayerSpeechBubble>();
            }
        }
    }
}
