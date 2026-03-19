using UnityEngine;

public class PortalTeleport : MonoBehaviour
{
    public Transform exitPoint;
    public KeyCode interactKey = KeyCode.W;
    public bool requireKeyPress = true;

    private bool playerInRange = false;
    private Transform player;

    void Update()
    {
        if (!playerInRange || player == null) return;

        if (requireKeyPress)
        {
            if (Input.GetKeyDown(interactKey))
            {
                TeleportPlayer();
            }
        }
        else
        {
            TeleportPlayer();
        }
    }

    void TeleportPlayer()
    {
        player.position = exitPoint.position;

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        playerInRange = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            player = other.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            player = null;
        }
    }
}