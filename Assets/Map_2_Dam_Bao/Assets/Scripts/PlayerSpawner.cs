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

        Instantiate(selectedPrefab, spawnPoint.position, Quaternion.identity);
    }
}