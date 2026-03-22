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

    [Header("Unlock Message")]
    public TextMeshProUGUI unlockMessageText;
    public float unlockMessageDuration = 2f;

    private Coroutine unlockMessageCoroutine;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (playerController == null)
            playerController = FindFirstObjectByType<PlayerController>();

        if (playerHealth == null)
            playerHealth = FindFirstObjectByType<PlayerHealth>();

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
        if (pausePanel != null) pausePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void ShowGameOver()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ContinueFromDeathPoint()
    {
        if (playerController == null)
            playerController = FindFirstObjectByType<PlayerController>();

        if (playerHealth == null)
            playerHealth = FindFirstObjectByType<PlayerHealth>();

        if (playerController == null || playerHealth == null) return;

        Vector3 respawnPos = playerController.GetRespawnPoint();

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        Time.timeScale = 1f;
        playerHealth.RestoreFullHealthAndMana();
        playerController.ReviveAt(respawnPos);
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
}