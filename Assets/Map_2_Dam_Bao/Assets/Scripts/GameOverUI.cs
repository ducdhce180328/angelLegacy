using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameOverUI : MonoBehaviour
{
    public Health playerHealth;
    public Mana playerMana;
    public GameObject gameOverPanel;

    private bool isShown = false;
    private PlayerController playerController;
    private PlayerCombat playerCombat;
    private Rigidbody2D rb;

    void Start()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    void Update()
    {
        FindPlayerIfNeeded();

        if (!isShown && playerHealth != null && playerHealth.isDead)
        {
            isShown = true;
            StartCoroutine(ShowGameOverAfterDelay());
        }
    }

    IEnumerator ShowGameOverAfterDelay()
    {
        yield return new WaitForSeconds(1.5f);

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

        if (playerHealth == null) return;

        playerHealth.transform.position = CheckpointManager.respawnPosition;

        playerHealth.isDead = false;
        playerHealth.currentHP = playerHealth.maxHP;

        if (playerMana != null)
            playerMana.currentMana = playerMana.maxMana;

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        if (playerController != null)
            playerController.enabled = true;

        if (playerCombat != null)
            playerCombat.enabled = true;

        Animator anim = playerHealth.GetComponent<Animator>();
        if (anim != null)
        {
            anim.ResetTrigger("Death");
            anim.Play("Idle");
        }
    }

    void FindPlayerIfNeeded()
    {
        if (playerHealth != null && playerMana != null && playerController != null && playerCombat != null && rb != null)
            return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        if (playerHealth == null)
            playerHealth = player.GetComponent<Health>();

        if (playerMana == null)
            playerMana = player.GetComponent<Mana>();

        if (playerController == null)
            playerController = player.GetComponent<PlayerController>();

        if (playerCombat == null)
            playerCombat = player.GetComponent<PlayerCombat>();

        if (rb == null)
            rb = player.GetComponent<Rigidbody2D>();
    }
}