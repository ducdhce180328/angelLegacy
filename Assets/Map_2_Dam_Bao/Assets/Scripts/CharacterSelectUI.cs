using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelectUI : MonoBehaviour
{
    [Header("Character Prefabs")]
    public GameObject knightPrefab;
    public GameObject magePrefab;
    public GameObject roguePrefab;

    [Header("Scene Name")]
    public string gameSceneName = "scene2";

    [Header("Visual Effect")]
    public CharacterSelectVisual characterSelectVisual;

    public void SelectKnight()
    {
        if (CharacterSelectionManager.Instance != null)
        {
            CharacterSelectionManager.Instance.SelectCharacter(knightPrefab);
            Debug.Log("Da chon Knight");

            if (characterSelectVisual != null)
            {
                characterSelectVisual.HighlightKnight();
            }
        }
    }

    public void SelectMage()
    {
        if (CharacterSelectionManager.Instance != null)
        {
            CharacterSelectionManager.Instance.SelectCharacter(magePrefab);
            Debug.Log("Da chon Mage");

            if (characterSelectVisual != null)
            {
                characterSelectVisual.HighlightMage();
            }
        }
    }

    public void SelectRogue()
    {
        if (CharacterSelectionManager.Instance != null)
        {
            CharacterSelectionManager.Instance.SelectCharacter(roguePrefab);
            Debug.Log("Da chon Rogue");

            if (characterSelectVisual != null)
            {
                characterSelectVisual.HighlightRogue();
            }
        }
    }

    public void PlayGame()
    {
        if (CharacterSelectionManager.Instance == null)
        {
            Debug.LogWarning("Khong tim thay CharacterSelectionManager");
            return;
        }

        if (CharacterSelectionManager.Instance.selectedCharacterPrefab == null)
        {
            Debug.LogWarning("Chua chon nhan vat");
            return;
        }

        SceneManager.LoadScene(gameSceneName);
    }
}