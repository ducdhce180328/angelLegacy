using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameOverUI : MonoBehaviour
{
    public Health playerHealth;
    public Mana playerMana;
    public GameObject gameOverPanel;

    private bool isShown = false;
    private GameObject player;

    void Start()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    void Update()
    {
        FindPlayerIfNeeded();

        if (!isShown && PlayerCompatibilityUtility.IsDead(player))
        {
            isShown = true;
            StartCoroutine(ShowGameOverAfterDelay());
        }
    }

    IEnumerator ShowGameOverAfterDelay()
    {
        yield return new WaitForSecondsRealtime(1.5f);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        Time.timeScale = 0f;
    }

    public void PlayAgain()
    {
        Time.timeScale = 1f;

        if (!CheckpointManager.hasCheckpoint)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            return;
        }

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        RespawnPlayerAtCheckpoint();
        isShown = false;
    }

    void RespawnPlayerAtCheckpoint()
    {
        FindPlayerIfNeeded();

        PlayerCompatibilityUtility.Respawn(player, CheckpointManager.respawnPosition);
    }

    void FindPlayerIfNeeded()
    {
        if (player != null)
            return;

        player = PlayerCompatibilityUtility.FindPlayer();
        if (player == null) return;

        playerHealth = player.GetComponent<Health>();
        playerMana = player.GetComponent<Mana>();
    }
}
