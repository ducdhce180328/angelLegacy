using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private bool isActivated = false;
    private SpriteRenderer sr;

    public Color activatedColor = Color.green;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isActivated) return;

        if (other.CompareTag("Player"))
        {
            CheckpointManager.SetCheckpoint(transform.position + new Vector3(0f, 0.5f, 0f));
            isActivated = true;

            if (sr != null)
                sr.color = activatedColor;
        }
    }
}