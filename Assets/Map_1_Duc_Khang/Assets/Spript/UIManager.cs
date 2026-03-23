using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("Panels")]
    public GameObject startPanel;
    public GameObject displayPanel;
    public GameObject pausePanel;
    public GameObject gameOverPanel;

    [Header("Refs")]
    public PlayerController playerController;
    public PlayerHealth playerHealth;
    public ScoreManager scoreManager;
    public GameObject defaultPlayerPrefab;
    public Vector3 defaultSpawnPosition = new Vector3(1f, 1f, 0f);

    [Header("Unlock Message")]
    public TextMeshProUGUI unlockMessageText;
    public float unlockMessageDuration = 2f;

    private Coroutine unlockMessageCoroutine;
    private GameObject playerObject;

    private void Awake()
    {
        instance = this;
        ResolveSceneUIReferences();
    }

    private void Start()
    {
        ResolveSceneUIReferences();
        EnsureScenePlayer();
        RefreshPlayerReferences();

        if (scoreManager == null)
            scoreManager = FindFirstObjectByType<ScoreManager>();

        if (displayPanel != null) displayPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        if (unlockMessageText != null)
            unlockMessageText.gameObject.SetActive(false);

        if (startPanel != null && startPanel.activeSelf)
            Time.timeScale = 0f;
        else
            Time.timeScale = 1f;
    }

    private void EnsureScenePlayer()
    {
        if (PlayerCompatibilityUtility.FindPlayer() != null) return;

        GameObject playerPrefab = ResolveScenePlayerPrefab();
        if (playerPrefab == null)
        {
            Debug.LogWarning("Khong co player prefab hop le de spawn trong Scene_Duc");
            return;
        }

        Vector3 spawnPosition = PlayerSpawnData.hasSpawnPosition
            ? PlayerSpawnData.spawnPosition
            : defaultSpawnPosition;

        GameObject spawnedPlayer = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
        ConfigureSpawnedPlayer(spawnedPlayer);

        playerObject = spawnedPlayer;
        playerController = spawnedPlayer.GetComponent<PlayerController>();
        playerHealth = spawnedPlayer.GetComponent<PlayerHealth>();
        PlayerSpawnData.hasSpawnPosition = false;

        CheckpointManager.SetCheckpoint(spawnPosition);

        BindCameraToPlayer();
    }

    private GameObject ResolveScenePlayerPrefab()
    {
        if (CharacterSelectionManager.Instance != null)
        {
            GameObject selectedPrefab = CharacterSelectionManager.Instance.selectedCharacterPrefab;
            if (selectedPrefab != null)
            {
                return selectedPrefab;
            }
        }

        return defaultPlayerPrefab;
    }

    private void ConfigureSpawnedPlayer(GameObject player)
    {
        if (player == null) return;

        PlayerMovementController modernMovement = player.GetComponent<PlayerMovementController>();
        PlayerCombat modernCombat = player.GetComponent<PlayerCombat>();
        PlayerCombat_BuffHeal modernBuffHealCombat = player.GetComponent<PlayerCombat_BuffHeal>();
        PlayerController legacyController = player.GetComponent<PlayerController>();

        if (modernMovement != null && legacyController != null)
        {
            legacyController.enabled = false;
        }

        if (modernCombat != null)
        {
            modernCombat.enabled = true;
        }

        if (modernBuffHealCombat != null)
        {
            modernBuffHealCombat.enabled = true;
        }
    }

    private void RefreshPlayerReferences()
    {
        playerObject = PlayerCompatibilityUtility.FindPlayer();
        playerController = playerObject != null ? playerObject.GetComponent<PlayerController>() : null;
        playerHealth = playerObject != null ? playerObject.GetComponent<PlayerHealth>() : null;

        BindCameraToPlayer();
    }

    private void BindCameraToPlayer()
    {
        if (playerObject == null)
        {
            playerObject = PlayerCompatibilityUtility.FindPlayer();
        }

        if (playerObject == null) return;

        CameraFollow cameraFollow = FindFirstObjectByType<CameraFollow>();
        if (cameraFollow != null)
        {
            cameraFollow.target = playerObject.transform;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (startPanel != null && startPanel.activeSelf) return;
            if (gameOverPanel != null && gameOverPanel.activeSelf) return;

            if (displayPanel != null && displayPanel.activeSelf)
            {
                CloseDisplayPanel();
                return;
            }

            if (pausePanel != null && pausePanel.activeSelf)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void StartGame()
    {
        if (startPanel != null) startPanel.SetActive(false);
        if (displayPanel != null) displayPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        Time.timeScale = 1f;
    }

    public void OpenDisplayPanel()
    {
        if (displayPanel != null) displayPanel.SetActive(true);
    }

    public void CloseDisplayPanel()
    {
        if (displayPanel != null) displayPanel.SetActive(false);
    }

    public void PauseGame()
    {
        ResolveSceneUIReferences();
        if (pausePanel != null) pausePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        ResolveSceneUIReferences();
        if (pausePanel != null) pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void ShowGameOver()
    {
        ResolveSceneUIReferences();
        if (pausePanel != null) pausePanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ContinueFromDeathPoint()
    {
        RefreshPlayerReferences();

        if (playerObject == null) return;

        Vector3 respawnPos = CheckpointManager.hasCheckpoint
            ? CheckpointManager.respawnPosition
            : defaultSpawnPosition;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        Time.timeScale = 1f;
        PlayerCompatibilityUtility.Respawn(playerObject, respawnPos);
    }

    public void RestartFromBeginning()
    {
        Time.timeScale = 1f;

        if (scoreManager != null)
            scoreManager.ResetScore();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ShowUnlockMessage(string message)
    {
        if (unlockMessageText == null) return;

        if (unlockMessageCoroutine != null)
            StopCoroutine(unlockMessageCoroutine);

        unlockMessageCoroutine = StartCoroutine(ShowUnlockMessageRoutine(message));
    }

    private IEnumerator ShowUnlockMessageRoutine(string message)
    {
        unlockMessageText.text = message;
        unlockMessageText.gameObject.SetActive(true);

        yield return new WaitForSecondsRealtime(unlockMessageDuration);

        unlockMessageText.gameObject.SetActive(false);
    }

    private void ResolveSceneUIReferences()
    {
        if (pausePanel == null)
        {
            pausePanel = FindSceneObjectByName("PausePanel");
        }

        if (gameOverPanel == null)
        {
            gameOverPanel = FindSceneObjectByName("GameOverPanel");
        }
    }

    private GameObject FindSceneObjectByName(string objectName)
    {
        if (string.IsNullOrEmpty(objectName))
        {
            return null;
        }

        GameObject[] sceneObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject sceneObject in sceneObjects)
        {
            if (sceneObject == null || !sceneObject.scene.IsValid())
            {
                continue;
            }

            if (sceneObject.name == objectName)
            {
                return sceneObject;
            }
        }

        return null;
    }
}
